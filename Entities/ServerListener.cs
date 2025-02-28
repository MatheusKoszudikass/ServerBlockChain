using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class ServerListener
    {
        private readonly ConcurrentQueue<Socket> _pendingConnections = new();
        // private readonly SemaphoreSlim _connectionSemaphore = new(1, 1);
        private readonly Socket _socketServer = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly HashSet<Socket> _socketsServer = new();
        private Socket _socketClient = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public bool Listening { get; private set; }
        public uint Port { get; private set; }

        public ServerListener(uint port)
        {
            Port = port;
        }

        public void Start()
        {
            try
            {
                if (Listening) return;

                _socketServer.Bind(new IPEndPoint(IPAddress.Any, (int)Port));
                _socketServer.Listen(500); // Aumente o backlog para 500
                Listening = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao iniciar o servidor: {ex.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                _socketServer.Close();
                Listening = false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao parar o servidor: {ex.Message}");
            }
        }

        public async Task StartListeningForClients(CancellationToken cts)
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    _socketClient = await _socketServer.AcceptAsync(cts);

                    _pendingConnections.Enqueue(_socketClient);

                    ProcessPendingConnectionsAsync(cts);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Stopping listening to customers.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error when accepting customer: {ex.Message}");
                }
            }
        }

        private void ProcessPendingConnectionsAsync(CancellationToken cts)
        {
       
            try
            {
                while (_pendingConnections.TryDequeue(out var socket))
                {
                    string ipClient = socket.RemoteEndPoint?.ToString() ?? "Unknown";

                    lock (_socketsServer)
                    {
                        if (!_socketsServer.Any(c => c.RemoteEndPoint?.ToString() == ipClient))
                        {
                            _socketsServer.Add(socket);
                            OnClientConnectedAct(socket);
                            continue;
                        }

                        socket.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when processing pending connections: {ex.Message}");
            }
        }

        public async Task SendAcknowledgment()
        {
            try
            {
                const string ack = "OK";
                var ackBytes = Encoding.UTF8.GetBytes(ack);
                var lengthPrefix = BitConverter.GetBytes(ackBytes.Length);

                await _socketClient.SendAsync(lengthPrefix, SocketFlags.None);
                await _socketClient.SendAsync(ackBytes, SocketFlags.None);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending acknowledgment: {ex.Message}");
            }
        }

        public async Task StartConnectionMonitoring()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    if (!IsSocketConnected(_socketClient))
                    {
                        Console.WriteLine($"Client {_socketClient?.RemoteEndPoint?.ToString()} disconnected.");
                        OnDisconnectClient(_socketClient);
                        break;
                    }

                    await Task.Delay(5000, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    OnDisconnectClient(_socketClient);
                    break;
                }
            }
        }

        private static bool IsSocketConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        protected virtual void OnClientConnectedAct(Socket socket) => ClientConnectedAct?.Invoke(socket);
        protected virtual void OnDisconnectClient(Socket socket) => DisconnectClientAct?.Invoke(socket);

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        public event Action<Socket>? ClientConnectedAct;
        public event Action<Socket>? DisconnectClientAct;
        private const int CHECK_INTERVAL = 10000;
    }
}
