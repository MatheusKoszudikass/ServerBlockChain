using System.Net.Security;
using System.Net.Sockets;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Interface
{
    /// <summary>
    /// Defines a contract for monitoring and managing data transfer operations.
    /// Handles both secure and non-secure data transmission using sockets and SSL streams.
    /// </summary>
    /// <typeparam name="T">The type of data being monitored and transferred</typeparam>
    public interface IDataMonitorService<T> 
    {
        /// <summary>
        /// Initializes the service with socket and certificate for secure communication.
        /// Sets up the necessary dependencies for encrypted data transfer.
        /// </summary>
        /// <param name="socket">The client socket connection</param>
        /// <param name="certificate">The SSL certificate for secure communication</param>
        /// <param name="cts">Optional cancellation token to stop the operation</param>
        Task StartDepedenciesAsync(Socket socket, Certificate certificate, CancellationToken cts = default);

        /// <summary>
        /// Initializes the service with an established socket and SSL stream.
        /// Sets up the dependencies using pre-configured secure connection.
        /// </summary>
        /// <param name="socket">The client socket connection</param>
        /// <param name="sslStream">The configured SSL stream for secure communication</param>
        /// <param name="cts">Optional cancellation token to stop the operation</param>
        void StartDepedenciesAsync(Socket socket, SslStream sslStream, CancellationToken cts = default);

        /// <summary>
        /// Asynchronously receives data from the connected client.
        /// Handles single data item reception.
        /// </summary>
        /// <param name="cts">Optional cancellation token to stop the operation</param>
        Task ReceiveDataAsync(CancellationToken cts = default);

        /// <summary>
        /// Asynchronously receives a list of data from the connected client.
        /// Handles multiple data items reception.
        /// </summary>
        /// <param name="cts">Optional cancellation token to stop the operation</param>
        Task ReceiveListAsync(CancellationToken cts = default);

        /// <summary>
        /// Asynchronously sends data to the connected client.
        /// Handles single data item transmission.
        /// </summary>
        /// <param name="data">The data to be sent</param>
        /// <param name="cts">Optional cancellation token to stop the operation</param>
        Task SendDataAsync(T data, CancellationToken cts = default);

        /// <summary>
        /// Asynchronously sends a list of data to the connected client.
        /// Handles multiple data items transmission.
        /// </summary>
        /// <param name="data">The list of data to be sent</param>
        /// <param name="cts">Optional cancellation token to stop the operation</param>
        Task SendListDataAsync(List<T> data, CancellationToken cts = default);
    }
}