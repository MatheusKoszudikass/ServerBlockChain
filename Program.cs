using System.Net.WebSockets;
using ServerBlockChain;
using ServerBlockChain.Conection;
using ServerBlockChain.Connection;
using ServerBlockChain.InstructionsSocket;
class Program
{
    public static async Task Main(string[] args)
    {
        // var url = PoolP2p.Pool($"http://89.0.142.86:18082/json_rpc ",18082);

        // var deamonClient = await MoneroDaemonClient.CreateAsync(url.GetPool(), url.GetPort()).ConfigureAwait(false);

        // var result = await deamonClient.GetConnectionsAsync().ConfigureAwait(false);
       await SocketServer.StartAsync(5000);
       Console.ReadLine();
    }
}
