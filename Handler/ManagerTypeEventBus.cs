using System.Text.Json;
using ServerBlockChain.Entities;
using ServerBlockChain.Util;

namespace ServerBlockChain.Handler;

public class ManagerTypeEventBus()
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;

    public void PublishEventType(JsonElement data)
    {
        var obj = JsonElementConvert.ConvertToObject(data);
        if (obj == null) throw new ArgumentNullException(nameof(data));
        Console.WriteLine($"Type: {data.GetType().Name}");
        switch (obj)
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

    public void PublishListEventType(List<JsonElement> data)
    {
        var obj = new List<object>();

        foreach (var item in data)
        {
            obj.Add(JsonElementConvert.ConvertToObject(item));
        }

        if (obj == null || obj.Count == 0) throw new ArgumentNullException(nameof(data));
        Console.WriteLine($"Type List: {obj.GetType().Name}");

        if (obj.All(o => o is ClientMine))
            _globalEventBus.PublishList(obj.Cast<ClientMine>().ToList());
        else if (obj.All(o => o is ServerListener))
            _globalEventBus.PublishList(obj.Cast<ServerListener>().ToList());
        else if (obj.All(o => o is LogEntry))
            _globalEventBus.PublishList(obj.Cast<LogEntry>().ToList());
        else if (obj.All(o => o is SendMessageDefault))
            _globalEventBus.PublishList(obj.Cast<SendMessageDefault>().ToList());
        else if (obj.All(o => o is string))
            _globalEventBus.PublishList(obj.Cast<string>().ToList());
        else
            throw new ArgumentException("Unsupported data list type", nameof(data));
    }
}