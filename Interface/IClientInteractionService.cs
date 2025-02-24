using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Interface
{
    public interface IClientInteractionService
    {
        event Action ReturnToClientService;
        void Start(ClientMine clientMine);

        void ShowClientInfo();
        /// <summary>
        /// Sends a chat message to the client.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        void SendChatMessage(string message);

        /// <summary>
        /// Receives a chat message from the client.
        /// </summary>
        /// <returns>The received message.</returns>
        void ReceiveChatMessage();

        /// <summary>
        /// Sends a file to the client.
        /// </summary>
        /// <param name="filePath">The path of the file to be sent.</param>
        void SendFile(string filePath);

        /// <summary>
        /// Receives a file from the client.
        /// </summary>
        /// <param name="destinationPath">The path where the received file should be saved.</param>
        void ReceiveFile(string destinationPath);

        /// <summary>
        /// Sends an object to the client.
        /// </summary>
        /// <param name="obj">The object to be sent.</param>
        void SendObject(object obj);

        /// <summary>
        /// Receives an object from the client.
        /// </summary>
        /// <returns>The received object.</returns>
        object ReceiveObject();
    }
}