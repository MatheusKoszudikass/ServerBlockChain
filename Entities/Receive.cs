using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class Receive<T>(SslStream sslStream, CancellationTokenSource cancellationTokenSource)
    {
        private readonly SslStream _sslStream = sslStream;
        private int TotalBytesReceived = 0;
        public CancellationTokenSource _cancellationTokenSource = cancellationTokenSource;
        public event EventHandler<T>? Received;
        public event EventHandler<CancellationTokenSource>? CanceledOperation;
        public event EventHandler<SslStream>? ClientDisconnected;

        public async Task ReceiveDataAsync()
        {
            try
            {
                await ExecuteWithTimeout(() => ReceiveLengthPrefix(), TimeSpan.FromSeconds(5));
                await ExecuteWithTimeout(() => ReceiveObject(), TimeSpan.FromSeconds(5));

                DeserializeObject();
            }
            catch (SocketException ex)
            {
                throw new Exception($"Error receiving object: {ex.Message}");
            }
        }

        private async Task ReceiveLengthPrefix()
        {
            try
            {
                await _sslStream.ReadAsync(StateObject.BufferInit.AsMemory(0, StateObject.BufferInit.Length));
                StateObject.BufferReceiveSize = BitConverter.ToInt32(StateObject.BufferInit, 0);
                StateObject.BufferReceive = new byte[StateObject.BufferReceiveSize];
            }
            catch (Exception ex)
            {
                throw new Exception($"Error receiving length prefix: {ex.Message}");
            }
        }

        private async Task ReceiveObject()
        {
            TotalBytesReceived = 0;
            while (TotalBytesReceived < StateObject.BufferReceiveSize)
            {
                int bytesRead = await _sslStream.ReadAsync(
                    StateObject.BufferReceive.AsMemory(TotalBytesReceived,
                    StateObject.BufferReceiveSize - TotalBytesReceived), _cancellationTokenSource.Token);

                if (bytesRead == 0)
                {
                    OnClientDisconnected();
                    OnCanceledOperation(_cancellationTokenSource);
                    break;
                }
                TotalBytesReceived += bytesRead;
            }
        }

        private T? DeserializeObject()
        {
            try
            {
                if (this.TotalBytesReceived == StateObject.BufferReceiveSize)
                {
                    string jsonData = Encoding.UTF8.GetString(StateObject.BufferReceive);
                    var resultObj = JsonSerializer.Deserialize<T>(jsonData);
                    OnReceived(resultObj!);
                    return resultObj;
                }
                return default;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Error deserializing object: {ex.Message}");
            }
        }

        private async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout)
        {
            var timeoutTask = Task.Delay(timeout, _cancellationTokenSource.Token);
            var task = taskFunc();

            if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
                throw new TimeoutException("Operation timed out.");

            await task;
        }

        protected virtual void OnReceived(T data)
        {
            Received?.Invoke(this, data);
        }

        protected virtual void OnCanceledOperation(CancellationTokenSource cancellationTokenSource)
        {
            CanceledOperation?.Invoke(this, cancellationTokenSource);
        }

        protected virtual void OnClientDisconnected()
        {
            ClientDisconnected?.Invoke(this, this._sslStream);
        }
    }
}