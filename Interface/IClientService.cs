using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;

namespace ServerBlockChain.Interface
{
    public interface IClientService
    {
        void Start(ServerListener serverListener);
        void ConnectClient(ServerListener serverListener);
        void GetAllConnectedClients(TypeHelp type, ServerListener serverListener);
        void ShowClientInfo();
        void SelectClient();
        void DisconnectClient();
    }
}