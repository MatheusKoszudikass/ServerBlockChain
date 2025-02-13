using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class SocketClosedEventHandler (Socket socket) : EventArgs
    {
        public Socket ClosedSocket { get; private set; } = socket;
    }
}