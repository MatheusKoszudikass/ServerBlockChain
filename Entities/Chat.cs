using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class Chat (ClientMine clientMine)
    {
        public StateObject State { get; set; } = new();
        public ClientMine ClientMine { get; } = clientMine;
        public string? Message { get; private set; }

        public async Task Start()
        {
            try
            {
                Task listenTask = Listen();

                while (this.ClientMine.Status)
                {
                    await Send();
                }

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
                while (this.ClientMine.SocketClient.Connected)
                {
                    int bytesRead = await this.ClientMine.SocketClient.ReceiveAsync(State.Buffer, SocketFlags.None);

                    Console.WriteLine("> ");
                    if (bytesRead == 0)
                    {
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
                    await this.ClientMine.SocketClient.SendAsync(messageBytes, SocketFlags.None);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Erro ao enviar mensagem.");
            }
        }

        private void CloseConnection()
        {
            if (this.ClientMine.SocketClient.Connected)
            {
                this.ClientMine.SocketClient.Close();
                Console.WriteLine($"{ClientMine.Ip} conexão encerrada.");
            }
        }
    }
}
