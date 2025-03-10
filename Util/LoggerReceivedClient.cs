using System.Collections.Concurrent;
using System.Text.Json;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Handler;
using ServerBlockChain.Interface;
using ServerBlockChain.Service;

namespace ServerBlockChain.Util;

public class LoggerReceivedClient
{
    private readonly string _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), @"Logs/Client");
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;
    private readonly JsonSerializerOptions _json = new() { WriteIndented = true };
    private readonly int _maxFileSize = 10 * 1024 * 1024;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentQueue<List<LogEntry>> _logQueue = new();
    private readonly IILogger<LogEntry> _loggerService = new LoggerService<LogEntry>();

    public LoggerReceivedClient()
    {
        _globalEventBus.SubscribeList<LogEntry>(OnLogsReceived);
    }

    private void OnLogsReceived(List<LogEntry> logEntries)
    {
        if (logEntries.Count == 0) return;

        _logQueue.Enqueue(logEntries);

        Task.Run(ProcessLogQueue);


        Console.WriteLine($"Log Queue Size: {_logQueue.Count}");
    }

    private async Task ProcessLogQueue()
    {
        while (_logQueue.TryDequeue(out var logEntries))
        {
            var userName = logEntries.FirstOrDefault()?.UserName
                           ?? logEntries.FirstOrDefault()?.UserDomain ?? "UnknownUser";

            var userDirectory = Path.Combine(_baseDirectory, userName);

            CheckIfDirectoryExists(userDirectory);
            var logFileInfo = InfoFileLogJson(userDirectory, userName);
            await SerializeLogEntries(logEntries, logFileInfo);

            await Task.Yield();
        }
    }

    private LogFileInfo InfoFileLogJson(string userDirectory, string userName)
    {
        return new LogFileInfo
        {
            Date = DateTime.Now.ToString("dd-MM-yyyy"),
            FileName = $"log_{userName}_{DateTime.Now:dd-MM-yyyy}.json",
            Path = Path.Combine(userDirectory, $"log_{userName}_{DateTime.Now:dd-MM-yyyy}.json")
        };
    }

    private void CheckIfDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private async Task SerializeLogEntries(List<LogEntry> logEntries,
        LogFileInfo logFileInfo)
    {
        try
        {
            await _semaphore.WaitAsync();
            CheckIfDirectoryExists(Path.GetDirectoryName(logFileInfo.Path)!);

            var newEntriesJson = JsonSerializer.Serialize(logEntries, _json);
            if (File.Exists(logFileInfo.Path))
            {
                var fileInfo = new FileInfo(logFileInfo.Path);
                if (fileInfo.Length + newEntriesJson.Length > _maxFileSize)
                {
                    logFileInfo.FileName = $"log_{logEntries[0].UserName}_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.json";
                    logFileInfo.Path = Path.Combine(_baseDirectory, logFileInfo.FileName);
                    return;
                }

                using var stream = new FileStream(logFileInfo.Path, FileMode.Open,
                    FileAccess.ReadWrite);
                stream.Seek(-1, SeekOrigin.End);
                if (stream.ReadByte() == ']')
                {
                    stream.SetLength(stream.Length - 1);
                }

                newEntriesJson = $",{newEntriesJson.TrimStart('[').TrimEnd(']')}";
            }

            await File.AppendAllTextAsync(logFileInfo.Path, newEntriesJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error serializing log entries: {ex.Message}");
            _loggerService.Log(ex, "Error serializing log entries client", LogLevel.Error);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}