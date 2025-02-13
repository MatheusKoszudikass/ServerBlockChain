
using System.Net.Sockets;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Conection
{
    public class PoolP2p
    {

        public static PoolConection Pool(string pool, uint port)
        {
            var poolConection = new PoolConection(pool, port);
            poolConection.AddConection();
            return poolConection;

        }

        // private string processNetwork(PoolConection poolConection)
        // {
        //     using(TcpClient client = new ())
        //     using (NetworkStream stream = client.GetStream())
        //     {
        //         string loginRequest = 
        //     }
        // }
    }
}