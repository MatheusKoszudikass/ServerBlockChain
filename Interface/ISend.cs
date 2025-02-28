using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;

namespace ServerBlockChain.Interface
{
    public interface ISend<T>
    {
        event Action<T> SendingAtc;
        event Action<SslStream> ClientDisconnectedAtc;

        Task SendAsync(T data);
    }
}