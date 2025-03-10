using System.Text.Json;
using ServerBlockChain.Service;
using ServerBlockChain.Entities;
using ServerBlockChain.Util;

namespace ServerBlockChain.Handler;

public class ManagerTypeEventBus()
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;

    public void PublishEventType(object data)
    {
        switch (data)
        {
            case ClientMine clientMine:
                _globalEventBus.Publish(clientMine);
                break;
            case ServerListener listener:
                _globalEventBus.Publish(listener);
                break;
            case LogEntry logEntry:
                _globalEventBus.Publish(logEntry);
                break;
            case SendMessageDefault sendMessageDefault:
                _globalEventBus.Publish(sendMessageDefault);
                break;
            case string message:
                _globalEventBus.Publish(message);
                break;
            default:
                throw new ArgumentException("Unsupported data type", nameof(data));
        }
    }

    public void PublishListEventType<T>(List<T> data)
    {
        if (data == null || data.Count == 0) throw new ArgumentNullException(nameof(data));
        Console.WriteLine($"Type: {data.GetType().Name}");
        switch (data)
        {
            case List<ClientMine> ClientMines:
                _globalEventBus.PublishList(ClientMines);
                break;
            case List<ServerListener> ServerListener:
                _globalEventBus.PublishList(ServerListener);
                break;
            case List<LogEntry> logEntries:
                _globalEventBus.PublishList(logEntries);
                break;
            case List<SendMessageDefault> sendMessageDefaults:
                _globalEventBus.PublishList(sendMessageDefaults);
                break;
            case List<string> messagens:
                _globalEventBus.PublishList(messagens);
                break;
            default:
                throw new ArgumentException("Unsupported data list type", nameof(data));
        }
    }
}