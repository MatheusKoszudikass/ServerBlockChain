using System.Collections.Concurrent;
using System.Net.Sockets;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Handler;

public sealed class ListTaskQueue
{
    private static ListTaskQueue _listTaskQueue;
    
    public static ListTaskQueue InstanceValue => _listTaskQueue ??= new();
    
    private static readonly ConcurrentQueue<Socket> PedingConnections = new();
    private static bool CanAcceptTask => PedingConnections.Count < 100;

    private ListTaskQueue() { }
    
    public bool EnqueueTaskAsync(Socket socket)
    {
        if (!CanAcceptTask)
        {
            Console.WriteLine("Limite de conexões atingido. Aguardando conexão de clientes...");
            return false;
        }
        
        PedingConnections.Enqueue(socket);
        return true;
    }
    public void TryDequeueConnection(Socket socket)
    {
        PedingConnections.TryDequeue(out socket!);
    }

    public void CountTaskProgress()
    {
        Console.WriteLine($"Conexões pendentes: {PedingConnections.Count}");
    }

    public bool StatusTask()
    {
        return !CanAcceptTask;
    }   
}