
namespace ServerBlockChain.Entiites
{

    public class ClientMine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Ip { get; set; } = string.Empty;
        public string SO { get; set; } = string.Empty;
        public int HoursRunning { get; set; }

        public HardwareInfo Hardware { get; set; } = new HardwareInfo();
        public MiningStats Mining { get; set; } = new MiningStats();
    }
}

