using ServerBlockChain.Service;

namespace ServerBlockChain.Interface;

public interface IDataMonitorInit
{
    void Initialize<T>(IClientManager clientManager, IILogger<T> logger);
    DataMonitorService<T> GetMonitor<T>();
}
