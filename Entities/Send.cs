using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class Send<T> (Socket socket)
    {
        private readonly Socket _socket = socket ?? throw new ArgumentNullException(nameof(socket));
        public event EventHandler<T>? Sending;
        public event EventHandler<T>? Sent;

        public async Task SendAsync(T data)
        {
            try
            {
                await SendLengthPrefix(data);
                await SendObject(data);
            }
            catch (SocketException ex)
            {
                throw new Exception($"Error sending object: {ex.Message}");
            }
        }

        private async Task SendLengthPrefix(T data)
        {
            StateObject.BufferSend = BitConverter.GetBytes(JsonSerializer.SerializeToUtf8Bytes(data).Length);
            await _socket.SendAsync(StateObject.BufferSend, SocketFlags.None);
        }

        private async Task SendObject(T data)
        {
            StateObject.BufferSend = JsonSerializer.SerializeToUtf8Bytes(data);
            await _socket.SendAsync(StateObject.BufferSend, SocketFlags.None);
        }

        protected virtual void OnSending(T data)
        {
            Sending?.Invoke(this, data);
        }

        protected virtual void OnSent(T data)
        {
            Sent?.Invoke(this, data);
        }
    }
}