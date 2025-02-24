using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public static class StateObject
    {
        public static int BufferSize = 4;
        public static int BufferReceiveSize;
        public static byte[] BufferInit { get; set; } = new byte[BufferSize];
        public static byte[] BufferReceive { get; set; } = new byte[BufferReceiveSize];
        public static byte[] BufferSend { get; set; } = [];
        public static Socket? WorkSocket { get; set; }
    }
}