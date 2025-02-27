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
        /// <summary>
        /// Starts the client service with the specified server listener.
        /// </summary>
        /// <param name="serverListener">The server listener to be used.</param>
        void Start(ServerListener serverListener);

        Task ListenerConnectionClient(Socket socket);

        /// <summary>
        /// Connects a client to the server using the specified server listener.
        /// </summary>
        /// <param name="serverListener">The server listener to be used.</param>
        void ConnectClient();

        /// <summary>
        /// Retrieves and displays all connected clients based on the specified type and server listener.
        /// </summary>
        /// <param name="type">The type of display to be used.</param>
        /// <param name="serverListener">The server listener to be used.</param>
        void GetAllConnectedClients(TypeHelp type);

        /// <summary>
        /// Displays information about a specific client.
        /// </summary>
        void ShowClientInfo();

        /// <summary>
        /// Selects a client from the list of connected clients.
        /// </summary>
        void SelectClient();

        /// <summary>
        /// Disconnects a client from the server.
        /// </summary>
        void DisconnectClient(Socket socketClient);
    }
}