using System.Text.Json;
using ServerBlockChain.Entities;
using ServerBlockChain.Interface;
using ServerBlockChain.Entities.Enum;
using System.Collections.Concurrent;

namespace ServerBlockChain.Service;
public class LoggerService<T> : IILogger<T>
{
    private readonly List<LogEntryServer> _logEntries = [];
    private readonly string _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), @"Logs/Server");
    private readonly JsonSerializerOptions _jason = new() { WriteIndented = true };
    private const int MaxFilesSize = 10 * 1024 * 1024;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentQueue<List<LogEntryServer>> _logQueue = new();
    
    public LoggerService()
    {
        _ = Task.Run(ProcessLogQueue);
    }

    public void Log(T data, Exception exception, string message, LogLevel level)
    {
        var logEntry = new LogEntryServer
        {
            Level = level.ToString(),
            Message = message,
            ApplicationName = "Client",
            Exception = exception.ToString(),
            Source = data,
            StackTrace = exception.StackTrace!
        };

        _logEntries.Add(logEntry);

        SerializeLogEntries();
    }

    public void Log(T data, string message, LogLevel level)
    {
        var logEntry = new LogEntryServer
        {
            Level = level.ToString(),
            Message = message,
            ApplicationName = "Client",
            Exception = string.Empty,
            Source = data,
            StackTrace = string.Empty
        };

        _logEntries.Add(logEntry);

        SerializeLogEntries();
    }

    public void Log(object data, Exception exception, string message, LogLevel level)
    {
        var logEntry = new LogEntryServer
        {
            Level = level.ToString(),
            Message = message,
            ApplicationName = "Client",
            Exception = exception.ToString(),
            Source = data,
            StackTrace = exception.StackTrace!
        };

        _logEntries.Add(logEntry);

        SerializeLogEntries();
    }

    public void Log(object data, string message, LogLevel level)
    {
        var logEntry = new LogEntryServer
        {
            Level = level.ToString(),
            Message = message,
            ApplicationName = "Client",
            Exception = string.Empty,
            Source = data,
            StackTrace = string.Empty
        };

        _logEntries.Add(logEntry);

        SerializeLogEntries();
    }

    public void SerializeLogEntries()
    {
        LogsQueue();
    }

    private void LogsQueue()
    {
        _logQueue.Enqueue(_logEntries);
    }

    private async Task ProcessLogQueue()
    {
        while (true)
        {
            while (_logQueue.TryDequeue(out var logEntries))
            {
                CheckIfDirectoryExists();
                var logFileInfo = CreateFileLogJson();
                await LogsProcessReadingWritten(logEntries, logFileInfo);
                await Task.Yield();
            }

            await Task.Delay(100);
        }
    }

    private async Task LogsProcessReadingWritten(List<LogEntryServer> logEntries, LogFileInfo logFileInfo)
    {
        try
        {

            await _semaphore.WaitAsync();
            // if (_logEntries.Count < 3) return;

            var newEtriesJson = JsonSerializer.Serialize(logEntries, _jason);

            if (File.Exists(logFileInfo.Path))
            {
                var fileInfo = new FileInfo(logFileInfo.Path);
                if (fileInfo.Length + newEtriesJson.Length > MaxFilesSize)
                {
                    logFileInfo.FileName = $"log_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.json";
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

                newEtriesJson = $",{newEtriesJson.TrimStart('[').TrimEnd(']')}";
            }

            await File.AppendAllTextAsync(logFileInfo.Path, newEtriesJson + "]");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error serializing log entries remoto: {ex.Message}");
            Log(logEntries.Count, ex, "Error serializing log entries remoto", LogLevel.Error);
            throw;
        }
        finally
        {
            _semaphore.Release();
            _logEntries.Clear();
            _logQueue.Clear();
        }
    }

    private LogFileInfo CreateFileLogJson()
    {
        return new LogFileInfo()
        {
            Date = DateTime.Now.ToString("dd-MM-yyyy"),
            FileName = $"log_{DateTime.Now:dd-MM-yyyy}.json",
            Path = Path.Combine(_baseDirectory, $"log_{DateTime.Now:dd-MM-yyyy}.json")
        };
    }

    private void CheckIfDirectoryExists()
    {
        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
        }
    }
}