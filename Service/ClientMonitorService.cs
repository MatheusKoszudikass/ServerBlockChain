using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Handler;
using ServerBlockChain.Interface;
using static System.Threading.Tasks.Task;

namespace ServerBlockChain.Service;

public class ClientMonitorService : IClientMonitor
{
    private readonly IDataMonitorService<LogEntry> _dataMonitorService;
    private readonly IClientManager _clientManager;
    private readonly IILogger<LogEntryServer> _iLogger;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public ClientMonitorService(IDataMonitorService<LogEntry>
        dataMonitorService, IClientManager clientManager, IILogger<LogEntryServer> iLogger)
    {
        _iLogger = iLogger;
        _dataMonitorService = dataMonitorService;
        _clientManager = clientManager;
        Task.Run(() => ReceiveDataAsync());
    }

    public async Task OpenMonitorReceiveClient(CancellationToken cts = default)
    {
        await Run(async () => await ReceiveDataAsync(cts), cts);
    }

    private async Task ReceiveDataAsync(CancellationToken cancellationToken = default)
    {
        var clients = _clientManager.GetAllClientsNoCompleteInfo();
        foreach (var client in clients)
        {
            try
            {
                await _semaphoreSlim.WaitAsync(cancellationToken);
                if (!client.Socket!.Connected) continue;

                _dataMonitorService.StartDepedenciesAsync(client.Socket, client.SslStream!, cts: cancellationToken);

                using var ctsTimeout = new CancellationTokenSource();

                using var linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ctsTimeout.Token);

                await _dataMonitorService.ReceiveDataAsync(linkedCts.Token);
            }
            catch (Exception ex)
            {
                _iLogger.Log(client, ex, $"Client Log {ex.Message}", LogLevel.Error);
            }
            finally
            {
                _semaphoreSlim.Release();
                _iLogger.Log(client, $"Client disconnected {client}", LogLevel.Information);
            }
        }
    }
}