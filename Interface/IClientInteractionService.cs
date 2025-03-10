using ServerBlockChain.Entities;

namespace ServerBlockChain.Interface
{
    /// <summary>
    /// Defines the contract for client interaction operations in the server-client communication.
    /// This interface handles all forms of data exchange between server and client including
    /// messages, files, and objects.
    /// </summary>
    public interface IClientInteractionService
    {
        /// <summary>
        /// Event triggered when the client service needs to return to its initial state
        /// </summary>
        event Action ReturnToClientService;

        /// <summary>
        /// Initializes the client interaction service with a specific client instance
        /// </summary>
        /// <param name="clientMine">The client instance to interact with</param>
        void Start(ClientMine clientMine);

        /// <summary>
        /// Displays detailed information about the connected client
        /// This includes connection status, client ID, and other relevant metadata
        /// </summary>
        void ShowClientInfo();

        /// <summary>
        /// Sends a text message to the connected client
        /// </summary>
        /// <param name="message">The message content to be sent</param>
        Task SendChatMessageAsync(string message);

        /// <summary>
        /// Receives and processes an incoming message from the client
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
        Task ReceiveChatMessageAsync();

        /// <summary>
        /// Transfers a file to the connected client
        /// </summary>
        /// <param name="filePath">The complete path of the file to be sent</param>
        /// <returns>Task representing the asynchronous file transfer operation</returns>
        Task SendFileAsync(string filePath);

        /// <summary>
        /// Receives a file from the connected client and saves it to the specified location
        /// </summary>
        /// <param name="destinationPath">The path where the received file should be saved</param>
        /// <returns>Task representing the asynchronous file reception operation</returns>
        Task ReceiveFileAsync(string destinationPath);

        /// <summary>
        /// Serializes and sends an object to the connected client
        /// </summary>
        /// <param name="obj">The object to be serialized and sent</param>
        /// <typeparam name="T">The type of object being sent</typeparam>
        /// <returns>Task representing the asynchronous send operation</returns>
        Task SendObjectAsync<T>(T obj) where T : class;

        /// <summary>
        /// Receives and deserializes an object from the connected client
        /// </summary>
        /// <typeparam name="T">The expected type of the received object</typeparam>
        /// <returns>Task<T> representing the asynchronous receive operation and the deserialized object</returns>
        Task<T> ReceiveObjectAsync<T>() where T : class;
    }
}