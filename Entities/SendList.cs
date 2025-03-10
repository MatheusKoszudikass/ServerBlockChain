using System.Net.Security;
using System.Text.Json;

namespace ServerBlockChain.Entities;

public class SendList<T>(SslStream sslStream)
{
    private readonly SslStream SslStream = sslStream;
    private readonly StateObject Buffer = new();
    public event Action<List<T>>? SentListAtc;

    public async Task SendListAsync(List<T> listData, CancellationToken cts = default)
    {
        await ExecuteWithTimeout(() =>
        SendLengthPrefix(listData, cts),
         TimeSpan.FromSeconds(5), cts);

        await ExecuteWithTimeout(() =>
         SendObjectAsync(listData, cts),
         TimeSpan.FromSeconds(5), cts); 
         
        await SslStream.FlushAsync(cts);
    }
    private async Task SendLengthPrefix(List<T> listData, CancellationToken cts = default)
    {
        this.Buffer.BufferSend = BitConverter.GetBytes(JsonSerializer.SerializeToUtf8Bytes(listData).Length);
        await this.SslStream.WriteAsync(this.Buffer.BufferSend, cts);
    }

    private async Task SendObjectAsync(List<T> listData, CancellationToken cts = default)
    {
        this.Buffer.BufferSend = JsonSerializer.SerializeToUtf8Bytes(listData);
        await this.SslStream.WriteAsync(this.Buffer.BufferSend, cts);
        OnSentListAtc(listData);
    }

    private static async Task ExecuteWithTimeout(Func<Task> taskFunc,
     TimeSpan timeout, CancellationToken cts)
    {
        var timeoutTask = Task.Delay(timeout, cts);
        var task = taskFunc();

        if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
            throw new TimeoutException("Operation timed out.");

        await task;
    }

    private void OnSentListAtc(List<T> listData)
    {
        SentListAtc!.Invoke(listData);
    }
}