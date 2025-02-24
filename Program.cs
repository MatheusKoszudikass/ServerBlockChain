using ServerBlockChain.Interface;
using ServerBlockChain.Service;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    public static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IClientInteractionService, ClientInteractionService>()
            .AddSingleton<IClientService, ClientService>()
            .AddSingleton<IMenuDisplayService, MenuDisplayService>()
            .AddSingleton<IRemotoService, RemotoService>()
            .BuildServiceProvider();
        
        var remotoService = serviceProvider.GetService<IRemotoService>();
        remotoService?.CreateRemoto();
    }
}
