using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerBlockChain.Handler;
using ServerBlockChain.Service;

namespace ServerBlockChain.Entities;

public sealed class ServerListener(uint port)
{
    private readonly ConcurrentQueue<Socket> _pendingConnections = new();
    private readonly Socket _socketServer = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly List<Socket> _clientsAccept = [];
    private Socket? SocketClient {get; set;}
    public bool Listening { get; private set; }
    public readonly GlobalEventBus? GlobalEventBus = GlobalEventBus.InstanceValue;
    private readonly SemaphoreSlim _semaphore = new(50);
    public uint Port { get; private set; } = port;

    public void Start()
    {

        if (Listening) return;

        _socketServer.Bind(new IPEndPoint(IPAddress.Any, (int)Port));
        _socketServer.Listen(500);
        Listening = true;
    }


    public async Task StartListeningForClients(CancellationToken cts = default)
    {
        while (!cts.IsCancellationRequested)
        {
            try
            {
                await _semaphore.WaitAsync();

                SocketClient = await _socketServer.AcceptAsync(cts);

                // if (ListTaskQueue.InstanceValue.StatusTask())
                // {
                //     await Task.Delay(100, cts);
                //     ListTaskQueue.InstanceValue.CountTaskProgress();
                //     continue;
                // }
                // await Task.Delay(TimeSpan.FromMinutes(6), cts);

                _pendingConnections.Enqueue(SocketClient);
                // ListTaskQueue.InstanceValue.EnqueueTaskAsync(SocketClient);
                ProcessPendingConnectionsAsync();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Client não tem conexão aberta");
                SocketClient = await _socketServer.AcceptAsync(cts);
                _pendingConnections.Enqueue(SocketClient);

                ProcessPendingConnectionsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when accepting customer: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    private void ProcessPendingConnectionsAsync()
    {
        while (_pendingConnections.TryDequeue(out var socket))
        {
            lock (_clientsAccept)
            {
                _clientsAccept.Add(socket);
                OnClientConnectedAct(socket);
                _clientsAccept.Remove(socket);
            }
        }
    }

    public void Stop()
    {

        _socketServer.Close();
        Listening = false;
    }

    private void OnClientConnectedAct(Socket socket) => ClientConnectedAct?.Invoke(socket);
    public event Action<Socket>? ClientConnectedAct;
}