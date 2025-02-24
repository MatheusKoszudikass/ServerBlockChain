using System;
using System.Threading.Tasks;
using System.Net.WebSockets;
using ServerBlockChain;
using ServerBlockChain.Conection;
using ServerBlockChain.Connection;
using ServerBlockChain.InstructionsSocket;
using ServerBlockChain.Interface;
using ServerBlockChain.Service;
using ServerBlockChain.Dependency;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    public static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IClientService, ClientService>()
            .AddSingleton<IMenuDisplayService, MenuDisplayService>()
            .AddSingleton<IRemotoService, RemotoService>()
            .BuildServiceProvider();
        
        var remotoService = serviceProvider.GetService<IRemotoService>();
        remotoService?.CreateRemoto();
    
        Console.ReadLine();
    }
}
