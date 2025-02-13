using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        public byte[] Buffer { get; set; } = new byte[BufferSize];
        public Socket? WorkSocket { get; set; }
    }
}