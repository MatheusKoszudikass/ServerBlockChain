using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;

namespace ServerBlockChain.Interface
{
    public interface IChannelService<T>
    {
        event Action<T>? ReceivedAct;
        event Action<T>? SendAct;
        event Action<SslStream>? DisconnectClientAct;
        Task WriteAsync(T data);
        Task ReadASync();
        IAsyncEnumerable<T> ReadAllAsync();
    }
}