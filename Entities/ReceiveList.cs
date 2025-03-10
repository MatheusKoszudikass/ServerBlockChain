using System.Net.Security;
using System.Text;
using System.Text.Json;

namespace ServerBlockChain.Entities;

public class ReceiveList<T>(SslStream sslStream)
{
    private readonly SslStream SslStream = sslStream;
    private readonly StateObject Buffer = new();
    public event Action<List<T>>? ReceiveListAct;
    private int _totalBytesReceived;

    public async Task ReceiveListAsync(CancellationToken cts = default)
    {
        await ReceiveLengthPrefix(cts);
        await ReceiveObjectAsync(cts);
        DeserializeObject();
    }

    private async Task ReceiveLengthPrefix(CancellationToken cts = default)
    {
        _ = await this.SslStream.ReadAsync(this.Buffer.BufferInit, cts);
        this.Buffer.BufferSize = BitConverter.ToInt32(this.Buffer.BufferInit, 0);
        this.Buffer.BufferReceive = new byte[this.Buffer.BufferSize];
    }

    private async Task ReceiveObjectAsync(CancellationToken cts = default)
    {
        _totalBytesReceived = 0;
        while (_totalBytesReceived < this.Buffer.BufferSize)
        {
            var bytesRead = await this.SslStream.ReadAsync(
                this.Buffer.BufferReceive.AsMemory(_totalBytesReceived,
                    this.Buffer.BufferSize - _totalBytesReceived), cts);
            if (bytesRead == 0) break;
            _totalBytesReceived += bytesRead;
        }
    }

    private void DeserializeObject()
    {
        if (this._totalBytesReceived != this.Buffer.BufferSize) return;
        var jsonData = Encoding.UTF8.GetString(this.Buffer.BufferReceive, 0, _totalBytesReceived);
        var resultObj = JsonSerializer.Deserialize<List<T>>(jsonData);
        OnReceiveList(resultObj!);
    }

    private void OnReceiveList(List<T> listData) => ReceiveListAct!.Invoke(listData);
}