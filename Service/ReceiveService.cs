using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Channels;
using System.Threading.Tasks;
using ServerBlockChain.Entities;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class ReceiveService<T> (SslStream ssltream, CancellationTokenSource cancellationToken) : IReceive<T>
    {
        private readonly SslStream _sslStream = ssltream;
        private Receive<T>? _receive;
        private readonly CancellationToken _cancellationToken = cancellationToken.Token;  
        public event Action<T>? ReceivedAtc;
        public event Action<SslStream>? ClientDesconnectedAct;

        public async Task ReceiveDataAsync()
        {
          _receive = new Receive<T>(_sslStream, cancellationToken);

          _receive.ClientDisconnectedAct += (data) => ClientDesconnectedAct?.Invoke(data);
          _receive.ReceivedAct += (data) => OnReceivedAtc(data);

           await _receive.ReceiveDataAsync();
        }

        protected virtual void OnReceivedAtc(T data)
        {
            ReceivedAtc?.Invoke(data);
        }
        
    }
}