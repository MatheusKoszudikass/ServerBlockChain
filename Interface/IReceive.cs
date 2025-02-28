using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;

namespace ServerBlockChain.Interface
{
    public interface IReceive<T>
    {
        event Action<T>? ReceivedAtc;
        event Action<SslStream>?  ClientDesconnectedAct;

        Task ReceiveDataAsync();
    }
}