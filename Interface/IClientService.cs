using System.Net.Sockets;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;

namespace ServerBlockChain.Interface
{
    /// <summary>
    /// Defines a contract for managing client services in the server application.
    /// Handles client connections, information display, and disconnection operations.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Initializes the client service with the specified server listener.
        /// Sets up necessary configurations and prepares for client handling.
        /// </summary>
        /// <param name="serverListener">The server listener instance to handle connections</param>
        void Start(ServerListener serverListener);
        
        /// <summary>
        /// Establishes a new client connection to the server.
        /// Handles the connection process and initial setup.
        /// </summary>
        void ConnectClient();

        /// <summary>
        /// Retrieves and displays all connected clients based on the specified display type.
        /// Provides different visualization options for client information.
        /// </summary>
        /// <param name="type">The type of help/display format to use</param>
        void GetAllConnectedClients(TypeHelp type);

        /// <summary>
        /// Displays detailed information about the currently selected client.
        /// Shows connection status, client ID, and other relevant metadata.
        /// </summary>
        void ShowClientInfo();

        /// <summary>
        /// Allows selection of a specific client from the list of connected clients.
        /// Enables interaction with a particular client instance.
        /// </summary>
        void SelectClient();

        /// <summary>
        /// Terminates the connection with a specific client.
        /// Handles cleanup and resource release for the disconnected client.
        /// </summary>
        /// <param name="socketClient">The socket of the client to disconnect</param>
        void DisconnectClient(Socket socketClient);
    }
}