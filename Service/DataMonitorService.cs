using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json;
using ServerBlockChain.Entities;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class DataMonitorService<T>(IClientManager clientManager) : IDataMonitorService<T>
    {
        private readonly IClientManager _clientManager = clientManager;
        private Socket? _socket;
        private SslStream? _sslStream;
        private ChannelService<T>? _channel;
        private Certificate? _certificate;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private event Action<ClientInfo>? ClienfInfo;
        public event Action<T>? DataReceived;

        public async Task StartDepencenciesAsync(Socket socket, Certificate certificate)
        {
            try
            {
                _socket = socket;
                _sslStream = await AuthenticateServer.AuthenticateServerAsync(socket, certificate);

                lock (_clientManager)
                {
                    _clientManager.AddClient(socket, _sslStream);
                }

                _channel = new ChannelService<T>(_sslStream, _cancellationTokenSource);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during monitoring: {ex.Message}");
            }
        }

        public void StartDepencenciesAsync(Socket socket, SslStream sslStream)
        {
            try
            {
                _channel = new ChannelService<T>(sslStream, _cancellationTokenSource);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during monitoring: {ex.Message}");

            }
        }

        public async Task ReceiveDataAsync()
        {
            try
            {
                _channel!.DisconnectClientAct += (data) =>
                {
                    Console.WriteLine($"Client disconnected -=: {data}");
                    _clientManager.DisconnectClient(data);
                };

                _channel!.ReceivedAct += (data) =>
                {
                    Console.WriteLine($"Received data +=: {data}");
                    OnDataReceived(data);
                };

                await _channel!.ReadASync();

            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data: {ex.Message}");
            }
        }

        public async Task SendDataAsync(T data)
        {         
            _channel!.DisconnectClientAct += (data) =>
            {
                Console.WriteLine($"Client disconnected -=: {data}");
                _clientManager.DisconnectClient(data);
            };

            _channel!.SendAct += (data) =>
            {
                Console.WriteLine($"Sent data: {data}");
            };

            await _channel!.WriteAsync(data);
        }

        private void VerifyDependencies()
        {
            if (_socket == null || _sslStream == null)
                throw new Exception("Dependencies not initialized");
        }

        private bool VerifyDependenciesStart()
        {
            if (_socket != null || _sslStream != null || _certificate != null) return true;

            return false;
        }

        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        protected void OnDataReceived(T data)
        {
            DataReceived?.Invoke(data);
        }

    }
}