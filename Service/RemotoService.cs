using System.Net.Sockets;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Handler;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service;

public sealed class RemotoService(IMenuDisplayService menuDisplayService,
IClientService clientService, IDataMonitorService<ClientMine> dataMonitorService,
IClientMonitor clientMonitor,
IILogger<ServerListener> logger) : IRemotoService
{
    private readonly IMenuDisplayService _menuDisplayService = menuDisplayService;
    private readonly IClientService _clientService = clientService;
    private readonly IDataMonitorService<ClientMine> _dataMonitorService = dataMonitorService;
    private readonly IClientMonitor _clientMonitor = clientMonitor;
    private readonly IILogger<ServerListener> _logger = logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private static StatusTask _statusTask = new();
    private ServerListener? _workSocket;

    public void CreateRemoto()
    {
        try
        {
            _menuDisplayService.RegisterOption(0, () => StartRemoto());
            _menuDisplayService.RegisterOption(1, StopRemoto);

            string[] options =
            [
                "Interact with the server",
                    "Exit"
            ];
            _menuDisplayService.SelectedOption(TypeHelp.Select, options, options);
        }
        catch (StackOverflowException ex)
        {
            throw new StackOverflowException($"Error when creating the remote server. {ex.Message}");
        }
    }

    private void StartRemoto()
    {
        try
        {
            var cancellationTokenSource = new CancellationTokenSource().Token;
            var cts = cancellationTokenSource;

            string[] options = ["Enter the port to start the server or press ENTER to use port 5000:"];

            _menuDisplayService.DisplayMenu(TypeHelp.Menu, options);

            var port = _menuDisplayService.SelectedIndex();

            if (port == 0) port = 5000;

            _workSocket = new ServerListener(checked((uint)port));

            _workSocket.ClientConnectedAct += (socketClient) =>
            {
                _logger.Log(socketClient, "Client connected", LogLevel.Information);

                Task.Run(async () =>
                {
                    await OnClientConnected(socketClient);
                });
            };

            _workSocket.Start();
            Task.Run(() => _workSocket.StartListeningForClients(cts), cts);
            _clientService.Start(_workSocket);
        }
        catch (SocketException ex)
        {
            _logger.Log(_workSocket!, ex, "Erro when starting the remote server" + ex.Message, LogLevel.Error);
            throw new SocketException();
        }
    }

    private async Task OnClientConnected(Socket socket)
    {

        try
        {
            await _semaphore!.WaitAsync();
            _statusTask.Status = true;
            var certificate = new Certificate();
            certificate.LoadCertificateFromEnvironment();
            await _dataMonitorService.StartDepedenciesAsync(socket, certificate);
            // await _dataMonitorService.ReceiveDataAsync();
        }
        catch (SemaphoreFullException ex)
        {
            _logger.Log(
                socket, ex, "Error of competition to receive the clientmine object." + ex.Message,
                 LogLevel.Error);

            throw;
        }
        finally
        {
            _semaphore.Release();
            // await OpenMonitorReceiveClient();
        }
    }

    private async Task OpenMonitorReceiveClient(CancellationToken cts = default)
    {
        try
        {
            await _semaphore.WaitAsync(cts);
            await _clientMonitor.OpenMonitorReceiveClient(cts);

        }
        catch (SemaphoreFullException ex)
        {
            _logger.Log(_workSocket!, ex, "Error of competition to receive the logEntry object." + ex.Message, LogLevel.Error);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void StopRemoto()
    {
        try
        {
            _workSocket?.Stop();
            Console.WriteLine("Stop remote server.");
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error when stopping the remote server: {ex.Message}");
        }
    }
}
