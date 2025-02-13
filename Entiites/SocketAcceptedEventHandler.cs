using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerBlockChain.Entiites
{
    public class SocketAcceptedEventHandler(Socket socket) : EventArgs
    {
        public Socket AcceptedSocket { get; private set; } = socket;
    }
}