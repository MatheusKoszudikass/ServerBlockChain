using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using ServerBlockChain.Handler;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service;

public class SerializationService<T> : ISerialization<T>
{
    private readonly ByteArrayEventBus __byteArrayEventBus;
    private readonly ManagerTypeEventBus _managerTypeEventBus = new();

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SerializationService(ByteArrayEventBus byteArrayEventBus)
    {
        __byteArrayEventBus = ByteArrayEventBus.InstanceValue;
        __byteArrayEventBus.Subscribe(messageByte => FromBytes(messageByte));
    }

    public byte[] ToBytes(T data)
    {
        try
        {
            return JsonSerializer.SerializeToUtf8Bytes(data, Options);
        }
        catch (Exception ex)
        {
            throw new SerializationException($"Failed to serialize {typeof(T).Name}", ex);
        }
    }

    public T FromBytes(byte[] data)
    {
        try
        {
            var json = Encoding.UTF8.GetString(data);
            var result = JsonSerializer.Deserialize<T>(json)
                         ?? throw new SerializationException($"Deserialization resulted in null for {typeof(T).Name}");
            _managerTypeEventBus.PublishEventType(result);
            return result;
        }
        catch (Exception ex)
        {
            throw new SerializationException($"Failed to deserialize {typeof(T).Name}", ex);
        }
    }
}