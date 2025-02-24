
using System.Net.Sockets;

namespace ServerBlockChain.Entities
{

    public class ClientMine(Socket socketClient)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Ip { get; set; } = string.Empty;
        public Socket SocketClient { get; set; } = socketClient;
        public bool Status { get; set; }
        public string SO { get; set; } = string.Empty;
        public int HoursRunning { get; set; }
        public HardwareInfo Hardware { get; set; } = new HardwareInfo();
        public MiningStats Mining { get; set; } = new MiningStats();
    }
}

