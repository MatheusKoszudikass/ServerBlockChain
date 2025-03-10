using System.Net.Security;
using System.Text.Json;
using ServerBlockChain.Entities;
using ServerBlockChain.Handler;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service;

public sealed class ReceiveService<T> : IReceive<T>
{
    private readonly SslStream _sslStream;
    private readonly ManagerTypeEventBus _managerTypeEventBus = new();
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public ReceiveService(SslStream ssltream)
    {
        _sslStream = ssltream;
        Task.Run(() => ReceiveDataAsync());
    }
    public async Task ReceiveDataAsync(CancellationToken cts = default)
    {
        try
        {
            while (!cts.IsCancellationRequested)
            {
                Console.WriteLine($"{_semaphoreSlim.CurrentCount}");
                var receive = new Receive<T>(_sslStream, cts);

                receive.ReceivedAct += OnReceivedAtc;
                receive.OnReceivedListAct += OnReceiveList;

                await receive.ReceiveDataAsync();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving data {ex.Message}");
            throw;
        }
    }

    public async Task ReceiveListDataAsync(CancellationToken cts = default)
    {
        var receiveList = new ReceiveList<T>(_sslStream);
        await receiveList.ReceiveListAsync(cts);
    }

    private void OnReceivedAtc(JsonElement data)
    {
        Console.WriteLine($"Data received{data}");
        _managerTypeEventBus.PublishEventType(data!);
    }

    private void OnReceiveList(List<JsonElement> listData)
    {
        Console.WriteLine($"List received {listData}");
        _managerTypeEventBus.PublishListEventType(listData);
    }
}
