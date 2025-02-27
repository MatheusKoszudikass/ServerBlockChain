using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerBlockChain.Entities
{
    public class ServerListener(uint port)
    {
        Socket SocketServer = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        readonly HashSet<Socket> SocketsServer = [];
        private Socket SocketClient = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public bool Listening { get; private set; }
        public uint Port { get; private set; } = port;

        public void Start()
        {
            try
            {
                if (this.Listening) return;

                this.SocketServer.Bind(new IPEndPoint(IPAddress.Any, (int)this.Port));
                this.SocketServer.Listen(200);
                this.Listening = true;

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
                this.SocketServer.Close();
                this.Listening = false;
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
                    SocketClient = await this.SocketServer.AcceptAsync(cts);
                    string ipClient = SocketClient.RemoteEndPoint?.ToString() ?? "Unknown";

                    lock (this.SocketsServer)
                    {
                        if (!this.SocketsServer.Any(c => c.RemoteEndPoint?.ToString() == ipClient))
                        {
                            this.SocketsServer.Add(SocketClient);
                            OnClientConnected(SocketClient);
                            continue;
                        }

                        SocketClient.Close();
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Parando a escuta de clientes.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao aceitar cliente: {ex.Message}");
                }

            }
        }

        protected virtual void OnClientConnected(Socket socket) => this.ClientConnectedAct?.Invoke(socket);
        protected virtual void OnDisconnectClient(Socket socket) => this.DisconnectClientAct?.Invoke(socket);

        private readonly CancellationTokenSource _cancellationTokenSource = new ();
        public event Action<Socket>? ClientConnectedAct;
        public event Action<Socket>? DisconnectClientAct;
        private const int CHECK_INTERVAL = 10000;
    }
}
