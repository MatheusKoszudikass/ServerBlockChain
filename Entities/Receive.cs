using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ServerBlockChain.Entities
{
    public sealed class Receive<T>(SslStream sslStream, CancellationToken cancellationToken)
    {
        private readonly SslStream _sslStream = sslStream;
        private int _totalBytesReceived;
        private readonly CancellationToken _cancellationToken = cancellationToken;
        private readonly StateObject Buffer = new();
        public event Action<T>? ReceivedAct;
        public event Action<List<T>>? OnReceivedListAct;

        public async Task ReceiveDataAsync()
        {
            await ExecuteWithTimeout(() => ReceiveLengthPrefix(), TimeSpan.FromSeconds(5));
            await ExecuteWithTimeout(() => ReceiveObject(), TimeSpan.FromSeconds(5));

            DeserializeObject();
        }

        private async Task ReceiveLengthPrefix()
        {
            _ = await this._sslStream.ReadAsync(this.Buffer.BufferInit, _cancellationToken);

            this.Buffer.BufferSize = BitConverter.ToInt32(this.Buffer.BufferInit, 0);
            this.Buffer.IsList = this.Buffer.BufferInit[4] == 1;

            this.Buffer.BufferReceive = new byte[this.Buffer.BufferSize];
        }

        private async Task ReceiveObject()
        {
            _totalBytesReceived = 0;
            while (_totalBytesReceived < this.Buffer.BufferSize)
            {
                var bytesRead = await _sslStream.ReadAsync(
                   this.Buffer.BufferReceive.AsMemory(_totalBytesReceived,
                       this.Buffer.BufferSize - _totalBytesReceived), _cancellationToken);

                if (bytesRead == 0) break;
                _totalBytesReceived += bytesRead;
            }
        }

        private void DeserializeObject()
        {
            if (this._totalBytesReceived != this.Buffer.BufferSize) return;
            var jsonData = Encoding.UTF8.GetString(this.Buffer.BufferReceive, 0, _totalBytesReceived);

            if (this.Buffer.IsList)
            {
                var resultList = JsonSerializer.Deserialize<List<T>>(jsonData);
                OnReceivedList(resultList!);
                return;
            }
            var resultObj = JsonSerializer.Deserialize<T>(jsonData);
            OnReceived(resultObj!);
        }

        private async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout)
        {
            var timeoutTask = Task.Delay(timeout, _cancellationToken);
            var task = taskFunc();

            if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
                throw new TimeoutException("Operation timed out.");

            await task;
        }

        private void OnReceived(T data)
        {
            ReceivedAct?.Invoke(data);
        }

        private void OnReceivedList(List<T> data)
        {
            OnReceivedListAct!.Invoke(data);
        }
    }
}