using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class Receive<T>
    {
        private  readonly Socket _socket;

        private  readonly SslStream _sslStream;

        private int TotalBytesReceived = 0;

        public CancellationTokenSource _cancellationTokenSource = new();

        public event EventHandler<T>? Receiving;
        public event EventHandler<T>? Received;
        public event EventHandler<CancellationTokenSource>? CanceledOperation;

        public Receive(Socket socket)
        {
            _socket = socket;
            _sslStream = new SslStream(new NetworkStream(_socket), false);
        }

        public async Task AuthenticateAsClientAsync(string targetHost)
        {
            await _sslStream.AuthenticateAsClientAsync(targetHost);
        }

        public async Task<T?> ReceiveDataAsync()
        {
            try
            {
                await ReceiveLengthPrefix();

                await ReceiveObject();

                return DeserializeObject();
            }
            catch (SocketException ex)
            {
                throw new Exception($"Error receiving object: {ex.Message}");
            }
        }

        private async Task ReceiveLengthPrefix()
        {
            await _sslStream.ReadAsync(StateObject.BufferInit, 0, StateObject.BufferInit.Length);
            StateObject.BufferReceiveSize = BitConverter.ToInt32(StateObject.BufferInit, 0);
            StateObject.BufferReceive = new byte[StateObject.BufferReceiveSize];
        }

        private async Task ReceiveObject()
        {
            TotalBytesReceived = 0;
            while (TotalBytesReceived < StateObject.BufferReceiveSize)
            {
                int bytesRead = await _sslStream.ReadAsync(
                    StateObject.BufferReceive.AsMemory(TotalBytesReceived,
                    StateObject.BufferReceiveSize - TotalBytesReceived), _cancellationTokenSource.Token);

                if (bytesRead == 0) break;
                TotalBytesReceived += bytesRead;
            }
        }

        private T? DeserializeObject()
        {
            try
            {
                if(this.TotalBytesReceived  == StateObject.BufferReceiveSize)
                {
                    string jsonData = Encoding.UTF8.GetString(StateObject.BufferReceive);
                    return JsonSerializer.Deserialize<T>(jsonData) ?? throw new ArgumentNullException(nameof(jsonData));
                }

                return default;
            }
            catch (JsonException ex)
            {

                throw new Exception($"Error deserializing object: {ex.Message}");
            }
        }

        protected virtual void OnReceiving(T data)
        {
            Receiving?.Invoke(this, data);
        }

        protected virtual void OnReceived(T data)
        {
            Received?.Invoke(this, data);
        }

        protected virtual void OnCanceledOperation(CancellationTokenSource cancellationTokenSource)
        {
            CanceledOperation?.Invoke(this, cancellationTokenSource);
        }
    }
}