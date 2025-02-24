using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class ClientMine(Socket socketClient)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string IpPublic { get; set; } = string.Empty;
        public string IpLocal { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string SO { get; set; } = string.Empty;
        public int HoursRunning { get; set; }
        public Socket SocketClient { get; set; } = socketClient;
        public HardwareInfomation? HardwareInfo { get; set; }
        public MiningStats Mining { get; set; } = new MiningStats();

        public async Task ReceiveData(CancellationToken cancellationToken)
        {
            try
            {
                {
                    while (SocketClient.Connected)
                    {
                        try
                        {
                            await this.SocketClient.ReceiveAsync(StateObject.BufferInit, SocketFlags.None, cancellationToken);
                            StateObject.BufferReceiveSize = BitConverter.ToInt32(StateObject.BufferInit, 0);

                            StateObject.BufferReceive = new byte[StateObject.BufferReceiveSize];
                            int totalBytesReceived = 0;
                            while (totalBytesReceived < StateObject.BufferReceiveSize)
                            {
                                int bytesRead = await this.SocketClient.ReceiveAsync(
                                    StateObject.BufferReceive.AsMemory(totalBytesReceived,
                                    StateObject.BufferReceiveSize - totalBytesReceived), SocketFlags.None, cancellationToken);

                                if (bytesRead == 0) break;
                                totalBytesReceived += bytesRead;
                            }

                            if (totalBytesReceived == StateObject.BufferReceiveSize)
                            {
                                ProcessReceivedData(totalBytesReceived);
                                break;
                            }
                        }
                        catch (SocketException ex)
                        {
                            throw new Exception($"Error receiving object: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when receiving the object: {ex.Message}");
            }
        }


        public async Task SendAcknowledgment()
        {
            try
            {
                string ack = "OK";
                byte[] ackBytes = Encoding.UTF8.GetBytes(ack);
                byte[] lengthPrefix = BitConverter.GetBytes(ackBytes.Length);

                await SocketClient.SendAsync(lengthPrefix, SocketFlags.None);
                await SocketClient.SendAsync(ackBytes, SocketFlags.None);
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
                    if (!IsSocketConnected(this.SocketClient))
                    {
                        Status = false;
                        Console.WriteLine($"Client {this.Id} disconnected.");
                        OnClientDisconnected();
                        break;
                    }

                    await SendAcknowledgment();
                    Status = true;
                    await Task.Delay(CHECK_INTERVAL, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    Status = false;
                    OnClientDisconnected();
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

        protected virtual void OnClientDisconnected()
        {
            DisconnectClient?.Invoke(this);
        }

        private void ProcessReceivedData(int totalBytesReceived)
        {
            try
            {
                var message = Encoding.UTF8.GetString(StateObject.BufferReceive, 0, totalBytesReceived);
                var resultObj = JsonSerializer.Deserialize<ClientMine>(message);

                MapperReceivedData(resultObj?? throw new ArgumentNullException(nameof(resultObj)));
                OnInfoClientMine(this ?? throw new ArgumentNullException(nameof(resultObj)));
                // _ = StartConnectionMonitoring();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when processing the object: {ex.Message}");
            }
        }

        private void MapperReceivedData(ClientMine clientMineReceived)
        {
            this.Id = clientMineReceived.Id;
            this.IpPublic = clientMineReceived.IpPublic;
            this.IpLocal = clientMineReceived.IpLocal;
            this.Name = clientMineReceived.Name;
            this.Status = clientMineReceived.Status;
            this.SO = clientMineReceived.SO;
            this.HoursRunning = clientMineReceived.HoursRunning;
            this.HardwareInfo = clientMineReceived.HardwareInfo;
        }

        protected virtual void OnInfoClientMine(ClientMine client)
        {
            InfoClientMine?.Invoke(client ?? throw new ArgumentNullException(nameof(client)));
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            SocketClient.Dispose();
        }

        public event Action<ClientMine>? DisconnectClient;
        public event Action<ClientMine>? InfoClientMine;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private const int CHECK_INTERVAL = 2000;

    }
}

