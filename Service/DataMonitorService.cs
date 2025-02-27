using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json;
using ServerBlockChain.Entities;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class DataMonitorService<T> (IClientManager clientManager) : IDataMonitorService<T>
    {
        private readonly IClientManager _clientManager = clientManager;
        private Socket? _socket;
        private SslStream? _auth;
        private Send<T>? _send;
        private Receive<T>? _receive;
        private Certificate? _certificate;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private event Action<ClientInfo>? ClienfInfo;
        public event Action<T>? DataReceived;

        public async Task StartDepencenciesAsync(Socket socket, Certificate certificate)
        {
            try
            {
                _socket = socket;
                _certificate = certificate;
                _auth = await AuthenticateServer.AuthenticateServerAsync(socket, certificate);
                _receive = new Receive<T>(_auth, _cancellationTokenSource);
                _send = new Send<T>(_auth, _cancellationTokenSource);

                _clientManager.AddClient(socket, _auth); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during monitoring: {ex.Message}");
                throw;
            }
        }

        public void StartDepencenciesAsync(Socket socket, SslStream sslStream)
        {
            try
            {
                _socket = socket;
                _receive = new Receive<T>(sslStream, _cancellationTokenSource);
                _send = new Send<T>(sslStream, _cancellationTokenSource);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during monitoring: {ex.Message}");
                throw;
            }
        }

        public async Task ReceiveDataAsync()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (_receive != null)
                    {
                        _receive!.ClientDisconnected += (sender, data) =>
                        {
                            Console.WriteLine($"Client disconnected -=: {data}");
                            _clientManager.DisconnectClient(data);

                        };
                        _receive!.Received += (sender, data) =>
                        {
                            Console.WriteLine($"Received data +=: {data}");
                            OnDataReceived(data);
                        };

                        await _receive.ReceiveDataAsync();
                    }
                    break;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Socket error: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving data: {ex.Message}");
                    break;
                }
            }
        }

        public async Task SendDataAsync(T data)
        {
            VerifyDependencies();
            _send!.Sending += (sender, data) => Console.WriteLine($"Sent data: {data}");
            await _send.SendAsync(data);
        }

        private void VerifyDependencies()
        {
            if (_socket == null || _auth == null || _receive == null || _send == null)
                throw new Exception("Dependencies not initialized");
        }

        private bool VerifyDependenciesStart()
        {
            if (_socket != null || _auth != null || _receive != null || _send != null || _certificate != null) return true;

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