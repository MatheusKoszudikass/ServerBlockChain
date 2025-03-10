using System.Net.Security;
using ServerBlockChain.Entities;
using ServerBlockChain.Handler;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class SendService<T>(SslStream sslStream) : ISend<T>
    {
        private readonly SslStream _sslStream = sslStream;
        private readonly ManagerTypeEventBus _managerTypeEventBus = new ManagerTypeEventBus();

        public async Task SendAsync(T data, CancellationToken cts = default)
        {
            var send = new Send<T>(_sslStream);
            send.SendingAtc += OnSent;

            await send.SendAsync(data, cts);
        }

        public async Task SendAsync(List<T> data, CancellationToken cts = default)
        {
             var sendList =  new SendList<T>(_sslStream);
             
             sendList.SentListAtc += OnSentListAtc;
             
             await sendList.SendListAsync(data, cts);
        }

        private void OnSent(T data)
        {
            lock (_managerTypeEventBus)
            {
                Console.WriteLine($"Data sent {data}");
                _managerTypeEventBus.PublishEventType(data!);
            }
        }
        
        private void OnSentListAtc(List<T> listData)
        {
            lock (_managerTypeEventBus)
            {
                Console.WriteLine($"List sent {listData}");
                _managerTypeEventBus.PublishEventType(listData);
            }
        }
    }
}