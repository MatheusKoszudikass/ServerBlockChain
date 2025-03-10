using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class ClientInfo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Socket? Socket { get; set; }
        public bool StatusInfoComplete { get; set; }
        public SslStream? SslStream { get; set; }
    }
}