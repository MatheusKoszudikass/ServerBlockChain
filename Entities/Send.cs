using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json;

namespace ServerBlockChain.Entities
{
    public class Send<T>(SslStream sslStream)
    {
        private readonly SslStream SslStream = sslStream;
        private readonly StateObject Buffer = new();
        public event Action<T>? SendingAtc;
        public event Action<T>? SentAtc;


        public async Task SendAsync(T data, CancellationToken cts = default)
        {
            await ExecuteWithTimeout(
                () => SendLengthPrefix(data, cts),
                 TimeSpan.FromSeconds(5), cts);

            await ExecuteWithTimeout(
                () => SendObject(data, cts
                ), TimeSpan.FromSeconds(5), cts);

            await SslStream.FlushAsync(cts);
        }

        private async Task SendLengthPrefix(T data, CancellationToken cts = default)
        {
            this.Buffer.BufferSend = BitConverter.GetBytes(
                JsonSerializer.SerializeToUtf8Bytes(data).Length);

            await SslStream.WriteAsync(this.Buffer.BufferSend, cts);

            OnSending(data);
        }

        private async Task SendObject(T data, CancellationToken cts = default)
        {
            this.Buffer.BufferSend = JsonSerializer.SerializeToUtf8Bytes(data);

            await SslStream.WriteAsync(this.Buffer.BufferSend, cts);

            OnSent(data);
        }

        private static async Task ExecuteWithTimeout(Func<Task> taskFunc,
         TimeSpan timeout, CancellationToken cts)
        {
            var timeoutTask = Task.Delay(timeout, cts);
            var task = taskFunc();

            if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
                throw new TimeoutException("Operation timed out.");

            await task;
        }

        private void OnSending(T data)
        {
            SendingAtc?.Invoke(data);
        }

        private void OnSent(T data)
        {
            SentAtc?.Invoke(data);
        }
    }
}