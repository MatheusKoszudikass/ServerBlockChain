using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using ServerBlockChain.Entities;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class SendService<T>(SslStream sslStream, CancellationTokenSource cancellationToken) : ISend<T>
    {
        private readonly SslStream _sslStream = sslStream;
        private readonly CancellationToken _cancellationToken = cancellationToken.Token;
        private Send<T>? _send;
        public event Action<T>? SendingAtc;
        public event Action<SslStream>? ClientDisconnectedAtc;
        
        public async Task SendAsync(T data)
        {
            _send = new Send<T>(_sslStream, cancellationToken);
            _send.ClientDisconnectedAtc += (data) => ClientDisconnectedAtc?.Invoke(data);
            _send.SendingAtc += (data) => SendingAtc?.Invoke(data);

            await _send.SendAsync(data);
        }
    }
}