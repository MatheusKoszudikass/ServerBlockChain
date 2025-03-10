using DotNetEnv;
using ServerBlockChain.Interface;
using ServerBlockChain.Service;
using Microsoft.Extensions.DependencyInjection;
using ServerBlockChain.Handler;
using ServerBlockChain.Util;

namespace ServerBlockChain;

internal abstract class Program
{
    public static void Main(string[] args)
    {
        Env.Load(".env");
        var serviceProvider = new ServiceCollection()
            .AddSingleton(ByteArrayEventBus.InstanceValue)
            .AddSingleton(GlobalEventBus.InstanceValue)
            .AddSingleton(typeof(ManagerTypeEventBus))
            .AddSingleton<IClientInteractionService, ClientInteractionService>()
            .AddSingleton<IClientManager, ClientManagerService>()
            .AddSingleton<IClientMonitor, ClientMonitorService>()
            .AddSingleton<IClientService, ClientService>()
            .AddSingleton(typeof(IDataMonitorService<>), typeof(DataMonitorService<>))
            .AddSingleton(typeof(ISerialization<>), typeof(SerializationService<>))
            .AddSingleton(typeof(IILogger<>), typeof(LoggerService<>))
            .AddSingleton<IMenuDisplayService, MenuDisplayService>()
            .AddSingleton(typeof(IReceive<>), typeof(ReceiveService<>))
            .AddSingleton<IRemotoService, RemotoService>()
            .AddSingleton(typeof(ISend<>), typeof(SendService<>))
            .BuildServiceProvider();

        var remotoService = serviceProvider.GetService<IRemotoService>();
        remotoService?.CreateRemoto();
        _ = new LoggerReceivedClient();
    }
}
