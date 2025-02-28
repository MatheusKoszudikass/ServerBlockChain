using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Security;
using System.Threading.Channels;
using System.Threading.Tasks;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class ChannelService<T> : IChannelService<T>
    {
        public event Action<T>? ReceivedAct;
        public event Action<T>? SendAct;
        public event Action<SslStream>? DisconnectClientAct;
        private readonly SslStream _sslStream;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly Channel<T> _channel;
        private SendService<T>? _send;
        private ReceiveService<T>? _receive;
        private SemaphoreSlim _semaphoreSlim = new(1, 1);

        public ChannelService(SslStream sslStream, CancellationTokenSource cancellationToken)
        {
            _sslStream = sslStream;
            _cancellationToken = cancellationToken;
            _receive = new ReceiveService<T>(_sslStream, _cancellationToken);
            _send = new SendService<T>(_sslStream, _cancellationToken);

            _channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = true
            });
        }

        public async Task WriteAsync(T data)
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            // Escreve diretamente no canal (fila) sem bloquear
              await _send!.SendAsync(data);
            // await _channel.Writer.WriteAsync(data);

            // // Processa o envio na fila, sem bloquear a adição de novas mensagens
            // await ProcessSendAsync();
        }

        private async Task ProcessSendAsync()
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync(_cancellationToken.Token))
                {
                    // Pega a próxima mensagem da fila
                    if (_channel.Reader.TryRead(out var item))
                    {
                        // Processa a mensagem de envio
                        await _send!.SendAsync(item);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                
            }
        }

        public async Task ReadASync()
        {
            try
            {
                _receive!.ReceivedAtc += (data) => ReceivedAct?.Invoke(data);
                await _receive!.ReceiveDataAsync();
                // await _channel.Reader.ReadAsync(_cancellationToken.Token);
            }
            catch (OperationCanceledException)
            {

            }
        }
        public async IAsyncEnumerable<T> ReadAllAsync()
        {
            while (await _channel.Reader.WaitToReadAsync(_cancellationToken.Token))
            {
                while (_channel.Reader.TryRead(out var item))
                {
                    yield return item;
                }
            }
        }
    }
}
