using System.Net.Security;
using System.Net.Sockets;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Interface
{
    /// <summary>
    /// Defines the contract for managing client connections and their lifecycle in the server application.
    /// Handles client addition, removal, updates, and retrieval operations.
    /// </summary>
    public interface IClientManager
    {
        /// <summary>
        /// Adds a new client to the manager using their socket and SSL stream
        /// </summary>
        /// <param name="socket">The client's socket connection</param>
        /// <param name="sslStream">The SSL stream for secure communication</param>
        void AddClient(Socket socket, SslStream sslStream);
        
        /// <summary>
        /// Updates the completion status of a client's information
        /// </summary>
        /// <param name="clientId">The unique identifier of the client</param>
        /// <param name="status">The new completion status</param>
        void UpdateClientInfoComplete(Guid clientId, bool status);
        
        /// <summary>
        /// Disconnects a client using their socket connection
        /// </summary>
        /// <param name="socket">The socket connection to disconnect</param>
        void DisconnectClient(Socket socket);

        /// <summary>
        /// Disconnects a client using their SSL stream
        /// </summary>
        /// <param name="sslStream">The SSL stream to disconnect</param>
        void DisconnectClient(SslStream sslStream);

        /// <summary>
        /// Removes a client from the manager using their unique identifier
        /// </summary>
        /// <param name="clientId">The unique identifier of the client to remove</param>
        void RemoveClient(Guid clientId);

        /// <summary>
        /// Removes a client from the manager using their socket connection
        /// </summary>
        /// <param name="socket">The socket connection of the client to remove</param>
        void RemoveClient(Socket socket);

        /// <summary>
        /// Removes a client from the manager using their SSL stream
        /// </summary>
        /// <param name="sslStream">The SSL stream of the client to remove</param>
        void RemoveClient(SslStream sslStream);

        /// <summary>
        /// Retrieves a client's information using their unique identifier
        /// </summary>
        /// <param name="clientId">The unique identifier of the client</param>
        /// <returns>The client information if found; null otherwise</returns>
        ClientInfo? GetClient(Guid clientId);

        /// <summary>
        /// Retrieves all connected clients' information
        /// </summary>
        /// <returns>An enumerable collection of all client information</returns>
        IEnumerable<ClientInfo> GetAllClients();

        /// <summary>
        /// Retrieves all clients that have incomplete information
        /// </summary>
        /// <returns>An enumerable collection of clients with incomplete information</returns>
        IEnumerable<ClientInfo> GetAllClientsNoCompleteInfo();

        /// <summary>
        /// Retrieves the most recently added client's information
        /// </summary>
        /// <returns>The last client's information if any exists; null otherwise</returns>
        ClientInfo? GetLastClient();
    }
}
