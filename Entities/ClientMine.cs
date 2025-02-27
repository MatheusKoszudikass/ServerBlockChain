using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ServerBlockChain.Entities
{
    public sealed class ClientMine()
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string IpPublic { get; set; } = string.Empty;
        public string IpLocal { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Socket? Socket;
        public bool Status { get; set; }
        public string So { get; set; } = string.Empty;
        public int HoursRunning { get; set; }
        public HardwareInfomation? HardwareInfo { get; set; }
        public MiningStats? Mining { get; set; }

        // public async Task SendAcknowledgment()
        // {
        //     try
        //     {
        //         const string ack = "OK";
        //         var ackBytes = Encoding.UTF8.GetBytes(ack);
        //         var lengthPrefix = BitConverter.GetBytes(ackBytes.Length);

        //         await SocketClient.SendAsync(lengthPrefix, SocketFlags.None);
        //         await SocketClient.SendAsync(ackBytes, SocketFlags.None);
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception($"Error sending acknowledgment: {ex.Message}");
        //     }
        // }

        // public async Task StartConnectionMonitoring()
        // {
        //     while (!_cancellationTokenSource.Token.IsCancellationRequested)
        //     {
        //         try
        //         {
        //             if (!IsSocketConnected(this.SocketClient))
        //             {
        //                 Status = false;
        //                 Console.WriteLine($"Client {this.Id} disconnected.");
        //                 OnClientDisconnected();
        //                 break;
        //             }

        //             await SendAcknowledgment();
        //             Status = true;
        //             await Task.Delay(CheckInterval, _cancellationTokenSource.Token);
        //         }
        //         catch (OperationCanceledException)
        //         {
        //             break;
        //         }
        //         catch (Exception)
        //         {
        //             Status = false;
        //             OnClientDisconnected();
        //             break;
        //         }

        //     }
        // }

        // private static bool IsSocketConnected(Socket socket)
        // {
        //     try
        //     {
        //         return !(socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0);
        //     }
        //     catch (SocketException)
        //     {
        //         return false;
        //     }
        // }

        // private void OnClientDisconnected()
        // {
        //     DisconnectClient?.Invoke(this);
        // }

        // private void ProcessReceivedData(int totalBytesReceived)
        // {
        //     try
        //     {
        //         var message = Encoding.UTF8.GetString(StateObject.BufferReceive, 0, totalBytesReceived);
        //         var resultObj = JsonSerializer.Deserialize<ClientMine>(message);

        //         MapperReceivedData(resultObj?? throw new ArgumentNullException(nameof(resultObj)));
        //         OnInfoClientMine(this ?? throw new ArgumentNullException(nameof(resultObj)));
        //         // _ = StartConnectionMonitoring();
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception($"Error when processing the object: {ex.Message}");
        //     }
        // }

        private void MapperReceivedData(ClientMine clientMineReceived)
        {
            this.Id = clientMineReceived.Id;
            this.IpPublic = clientMineReceived.IpPublic;
            this.IpLocal = clientMineReceived.IpLocal;
            this.Name = clientMineReceived.Name;
            this.Status = clientMineReceived.Status;
            this.So = clientMineReceived.So;
            this.HoursRunning = clientMineReceived.HoursRunning;
            this.HardwareInfo = clientMineReceived.HardwareInfo;
        }

        private void OnInfoClientMine(ClientMine client)
        {
            InfoClientMine?.Invoke(client ?? throw new ArgumentNullException(nameof(client)));
        }

        // public void Dispose()
        // {
        //     _cancellationTokenSource.Cancel();
        //     _cancellationTokenSource.Dispose();
        //     SocketClient.Dispose();
        // }

        public event Action<ClientMine>? DisconnectClient;
        public event Action<ClientMine>? InfoClientMine;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private const int CheckInterval = 2000;

    }
}

