using System.Net.Security;
using System.Net.Sockets;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service;

public class DataMonitorService<T>(IClientManager clientManager,
 IILogger<T> ilogger) : IDataMonitorService<T>
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IILogger<T> _logger = ilogger;
    private Socket? _socket;
    private SendService<T>? _sendService;
    private ReceiveService<T>? _receiveService;

    public async Task StartDepedenciesAsync(Socket socket,
        Certificate certificate, CancellationToken cts = default)
    {
        try
        {
            _socket = socket;
            var sslStream = await AuthenticateServer.AuthenticateServerAsync(socket, certificate);

            lock (_clientManager)
            {
                _clientManager.AddClient(socket, sslStream);
                _logger.Log(socket, "StartDependencies", LogLevel.Information);
            }

            _sendService = new SendService<T>(sslStream);
            _receiveService = new ReceiveService<T>(sslStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during init dependencies: {ex.Message}");
            DisconnectClient();
            _logger.Log(socket, ex,
                ex.Message + " Error during init dependencies", LogLevel.Error);
        }
    }

    public void StartDepedenciesAsync(Socket socket,
        SslStream sslStream, CancellationToken cts = default)
    {
        try
        {
            _sendService = new SendService<T>(sslStream);
            _receiveService = new ReceiveService<T>(sslStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during monitoring: {ex.Message}");
            _logger.Log(socket, ex,
                "Error during two init dependencies" + ex.Message, LogLevel.Error);
        }
    }

    public async Task ReceiveDataAsync(CancellationToken cts = default)
    {
        try
        {
            await _receiveService!.ReceiveDataAsync(cts);
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Socket error: {ex.Message}");
            DisconnectClient();
            _logger.Log(_socket!, ex, "Error SslStream  ", LogLevel.Error);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving data: {ex.Message}");
            DisconnectClient();
            _logger.Log(_socket!, ex, "Error receiving data" + ex.Message, LogLevel.Error);
        }
    }

    public async Task ReceiveListAsync(CancellationToken cts = default)
    {
        try
        {
            await _receiveService!.ReceiveListDataAsync(cts);
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Socket error: {ex.Message}");
            _logger.Log(_socket!, ex, "Error SslStream  ", LogLevel.Error);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving data: {ex.Message}");
            DisconnectClient();
            _logger.Log(_socket!, ex, "Error receiving data" + ex.Message, LogLevel.Error);
        }
    }

    public async Task SendDataAsync(T data, CancellationToken cts = default)
    {
        try
        {
            await _sendService!.SendAsync(data, cts);
        }
        catch (SocketException ex)
        {
            _logger.Log(_socket!, ex, "Error SendDataAsync data " + ex.Message, LogLevel.Error);
            DisconnectClient();
            Console.WriteLine("Socket error SendDataAsync");
            throw;
        }
    }

    public async Task SendListDataAsync(List<T> data, CancellationToken cts = default)
    {
        try
        {
            await _sendService!.SendAsync(data, cts);
        }
        catch (Exception ex)
        {
            _logger.Log(_socket!, ex, "Error SendDataAsync List<data> " + ex.Message, LogLevel.Error);
            DisconnectClient();
            Console.WriteLine(ex);
            throw;
        }
    }

    private void DisconnectClient()
    {
        _clientManager.DisconnectClient(_socket!);
    }
}