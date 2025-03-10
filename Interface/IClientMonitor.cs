namespace ServerBlockChain.Interface
{
    /// <summary>
    /// Defines a contract for monitoring clients connected to the server.
    /// Responsible for managing asynchronous data reception from clients.
    /// </summary>
    public interface IClientMonitor
    {
        /// <summary>
        /// Starts monitoring and receiving data from connected clients.
        /// Operates continuously until the cancellation token is triggered.
        /// </summary>
        /// <param name="cts">Cancellation token to stop monitoring when needed</param>
        /// <returns>Task representing the asynchronous monitoring operation</returns>
        Task OpenMonitorReceiveClient(CancellationToken cts = default);
    }
}