using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class Chat(ClientMine clientMine)
    {
        public ClientMine ClientMine { get; } = clientMine;
        public string? Message { get; private set; }

        public void Start()
        {
            try
            {
                Task listenTask = Task.Run(() => Listen());

                // while (this.ClientMine.Status)
                // {
                //     await Send();
                // }
            }
            catch (SocketException)
            {
                Console.WriteLine($"{ClientMine.IpPublic} desconectado devido a erro.");
            }
        }

        private async Task Listen()
        {
            try
            {
                while (true)
                {
                    var buffer = new byte[4];
                    await this.ClientMine.SocketClient.ReceiveAsync(buffer, SocketFlags.None);
                    var messageLength = BitConverter.ToInt32(buffer, 0);

                    buffer = new byte[messageLength];
                    int totalBytesReceived = 0;
                    while (totalBytesReceived < messageLength)
                    {
                        int bytesRead = await this.ClientMine.SocketClient.ReceiveAsync(
                         buffer.AsMemory(totalBytesReceived, messageLength - totalBytesReceived), SocketFlags.None);
                        if (bytesRead == 0) break;
                        totalBytesReceived += bytesRead;
                    }

                    if (totalBytesReceived == messageLength)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, messageLength);

                        var resultObj = JsonSerializer.Deserialize<ClientMine>(message);

                        OnInfoClientMine(resultObj ?? throw new ArgumentNullException(nameof(resultObj)));
                        break;
                    }
                }
            }
            catch (SocketException)
            {
                Console.WriteLine($"{ClientMine.IpPublic} desconectado devido a erro.");
            }
        }


        private void CloseConnection()
        {
            if (this.ClientMine.SocketClient.Connected)
            {
                this.ClientMine.SocketClient.Close();
                Console.WriteLine($"{ClientMine.IpPublic} conexão encerrada.");
            }
        }

        protected virtual void OnInfoClientMine(ClientMine client)
        {
            InfoClientMine?.Invoke(this, client);
        }
        public event EventHandler<ClientMine>? InfoClientMine;
    }
}
