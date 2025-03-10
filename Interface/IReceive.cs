namespace ServerBlockChain.Interface;

public interface IReceive<T>
{
    Task ReceiveDataAsync(CancellationToken cts = default);
    Task ReceiveListDataAsync(CancellationToken cts = default);
}