using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ServerBlockChain.Entities
{
    public sealed class ClientMine()
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string IpPublic { get; set; } = string.Empty;
        public string IpLocal { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string So { get; set; } = string.Empty;
        public int HoursRunning { get; set; }
        public HardwareInfomation? HardwareInfo { get; set; }
        public MiningStats? Mining { get; set; }
    }
}

