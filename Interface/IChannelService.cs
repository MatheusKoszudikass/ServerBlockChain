namespace ServerBlockChain.Interface
{
    /// <summary>
    /// Represents a communication channel for reading and writing data asynchronously.
    /// </summary>
    public interface IDataChannelService
    {
        /// <summary>
        /// Writes data asynchronously to the channel.
        /// </summary>
        /// <param name="data">The byte array containing the data to be written.</param>
        /// <param name="cts">A cancellation token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WriteAsync(byte[] data, CancellationToken cts = default);

        /// <summary>
        /// Reads data asynchronously from the channel.
        /// </summary>
        /// <param name="cts">A cancellation token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous read operation.</returns>
        Task ReadAsync(CancellationToken cts = default);
    }
}
