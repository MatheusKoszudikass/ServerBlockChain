using System;
using System.Net;
using System.Net.Sockets;

namespace ServerBlockChain.Entiites
{
    public class Listener(uint port)
    {
        Socket SocketServer = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public bool Listening { get; set; }

        public uint Port { get; set; } = port;

        public void Start()
        {
            try
            {
                if (this.Listening) return;

                this.SocketServer.Bind(new IPEndPoint(IPAddress.Any, (int)this.Port));
                this.SocketServer.Listen(0);

                // this.SocketServer.BeginAccept(Callback, null);
                this.Listening = true;

            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao iniciar o servidor: {ex.Message}");
            }
        }

        public async Task<Socket?> AcceptClientAsync()
        {
            try
            {
                return await this.SocketServer.AcceptAsync();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Erro ao aceitar cliente: {ex.Message}");
                throw;
            }
        }

        public void Stop()
        {
            if (!this.Listening) return;

            this.SocketServer.Close();
            this.SocketServer.Dispose();
            this.SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Listening = false;
        }
    }
}
