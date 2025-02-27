using DotNetEnv;
using ServerBlockChain.Interface;
using ServerBlockChain.Service;
using Microsoft.Extensions.DependencyInjection;
class Program
{
    public static void Main(string[] args)
    {
        Env.Load(".env");
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IClientInteractionService, ClientInteractionService>()
            .AddSingleton<IClientManager, ClientManagerService>()
            .AddSingleton<IClientMonitor, ClientMonitorService>()
            .AddSingleton<IClientService, ClientService>()
            .AddSingleton(typeof(IDataMonitorService<>), typeof(DataMonitorService<>))
            .AddSingleton<IMenuDisplayService, MenuDisplayService>()
            .AddSingleton<IRemotoService, RemotoService>()
            .BuildServiceProvider();

        var remotoService = serviceProvider.GetService<IRemotoService>();
        remotoService?.CreateRemoto();
    }
}
