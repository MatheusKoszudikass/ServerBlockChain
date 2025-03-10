public class ByteArrayEventBus
{
    private static readonly Lazy<ByteArrayEventBus> Instance = new(() => new ByteArrayEventBus());
    public static ByteArrayEventBus InstanceValue => Instance.Value;
    private readonly List<Action<byte[]>> _handlers = [];

    private ByteArrayEventBus() { }

    public void Subscribe(Action<byte[]> handler)
    {
        lock (_handlers)
        {
            if (!_handlers.Contains(handler))
            {
                _handlers.Add(handler);
            }
        }
    }

    public void Publish(byte[] data)
    {
        if (data == null) return;

        foreach (var handler in _handlers.ToList())
        {
            handler(data);
        }
    }

    public void Unsubscribe(Action<byte[]> handler)
    {
        lock (_handlers)
        {
            _handlers.Remove(handler);
        }
    }
}