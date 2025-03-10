using ServerBlockChain.Interface;

namespace ServerBlockChain.Service;

public class DataMonitorInitService : IDataMonitorInit
{
    private readonly Dictionary<Type, object> _monitors = new();

    public void Initialize<T>(IClientManager clientManager, IILogger<T> logger)
    {
        if (!_monitors.ContainsKey(typeof(T)))
        {
            var monitor = new DataMonitorService<T>(clientManager, logger);
            _monitors[typeof(T)] = monitor;
        }
    }

    public DataMonitorService<T> GetMonitor<T>()
    {
        return (DataMonitorService<T>)_monitors[typeof(T)];
    }
}
