using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerBlockChain.Entiites
{
    public class Chat
    {
        public StateObject State { get; set; } = new();
        public Socket WorkSocket { get; }
        public ClientMine ClientMine { get; }
        public string? Message { get; private set; }

        public Chat(Socket socket, ClientMine clientMine)
        {
            WorkSocket = socket;
            ClientMine = clientMine;
        }

        public async Task Start()
        {
            try
            {
                // Inicia escuta do cliente
                Task listenTask = Listen();

                // Enquanto o cliente estiver conectado, permite envio de mensagens
                while (WorkSocket.Connected)
                {
                    await Send();
                }

                // Aguarda finalização do Listen
                await listenTask;
            }
            catch (SocketException)
            {
                Console.WriteLine($"{ClientMine.Ip} desconectado devido a erro.");
            }
            finally
            {
                CloseConnection();
            }
        }

        private async Task Listen()
        {
            try
            {
                while (WorkSocket.Connected)
                {
                    int bytesRead = await WorkSocket.ReceiveAsync(State.Buffer, SocketFlags.None);
                    Console.WriteLine("> ");
                    if (bytesRead == 0)
                    {
                        // Cliente desconectado
                        Console.WriteLine($"{ClientMine.Ip} desconectado.");
                        break;
                    }

                    Message = Encoding.UTF8.GetString(State.Buffer, 0, bytesRead);
                    Console.WriteLine($"{ClientMine.Ip}: {Message}");
                }
            }
            catch (SocketException)
            {
                Console.WriteLine($"{ClientMine.Ip} desconectado devido a erro.");
            }
            finally
            {
                CloseConnection();
            }
        }

        private async Task Send()
        {
            try
            {
                Console.Write("> ");
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(input);
                    await WorkSocket.SendAsync(messageBytes, SocketFlags.None);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Erro ao enviar mensagem.");
            }
        }

        private void CloseConnection()
        {
            if (WorkSocket.Connected)
            {
                WorkSocket.Close();
                Console.WriteLine($"{ClientMine.Ip} conexão encerrada.");
            }
        }
    }
}
