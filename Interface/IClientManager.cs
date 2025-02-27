using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Interface
{
    public interface IClientManager
    {
        event Action<ClientInfo> AddedClientAct;
        public event Action<ClientInfo>? DisconnectedClientAtc;
        event Action<ClientInfo> RemovedClientAct;

        void AddClient(Socket socket, SslStream sslStream);

        void DisconnectClient(Socket socket);
        void DisconnectClient(SslStream sslStream);

        void RemoveClient(Guid clientId);
        void RemoveClient(Socket socket);
        void RemoveClient(SslStream sslStream);

        ClientInfo? GetClient(Guid clientId);
        IEnumerable<ClientInfo> GetAllClients();
        ClientInfo? GetLastClient();
    }

}
