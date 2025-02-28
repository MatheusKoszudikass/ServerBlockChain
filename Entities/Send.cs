using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class Send<T>(SslStream sslStream, CancellationTokenSource cancellationTokenSource)
    {
        private readonly SslStream _sslStream = sslStream;
        private readonly CancellationTokenSource _cancellationTokenSource = cancellationTokenSource;
        public event Action<T>? SendingAtc; // Mantemos o evento Sent
        public event Action<SslStream>? ClientDisconnectedAtc; // Novo evento para notificar desconexão

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task SendAsync(T data)
        {

            try
            {
                await ExecuteWithTimeout(() => SendLengthPrefix(data), TimeSpan.FromSeconds(5));
                await ExecuteWithTimeout(() => SendObject(data), TimeSpan.FromSeconds(5));
            }
            catch (SocketException ex)
            {
                throw new Exception($"Error sending object: {ex.Message}");
            }
            catch (Exception ex)
            {
                OnClientDisconnected();
                throw new Exception($"Error: {ex.Message}");
            }
        }

        private async Task SendLengthPrefix(T data)
        {
            await _semaphore.WaitAsync(_cancellationTokenSource.Token);
            try
            {
                byte[] dataBytes = JsonSerializer.SerializeToUtf8Bytes(data);
                byte[] lengthPrefix = BitConverter.GetBytes(dataBytes.Length);
                await _sslStream.WriteAsync(lengthPrefix, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                // Notifica que o cliente desconectou em caso de erro
                OnClientDisconnected();
                throw new Exception($"Error sending length prefix: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task SendObject(T data)
        {
            await _semaphore.WaitAsync(_cancellationTokenSource.Token);
            try
            {
                byte[] dataBytes = JsonSerializer.SerializeToUtf8Bytes(data);
                await _sslStream.WriteAsync(dataBytes, _cancellationTokenSource.Token);
                OnSent(data); // Notifica que os dados foram enviados
            }
            catch (Exception ex)
            {

                OnClientDisconnected();
                throw new Exception($"Error sending object: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
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

        protected virtual void OnSent(T data)
        {
            SendingAtc?.Invoke(data);
        }

        protected virtual void OnClientDisconnected()
        {
            ClientDisconnectedAtc?.Invoke(_sslStream); // Dispara o evento de desconexão
        }
    }
}