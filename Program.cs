using System.Net.WebSockets;
using ServerBlockChain;
using ServerBlockChain.Conection;
using ServerBlockChain.Connection;
using ServerBlockChain.InstructionsSocket;
class Program
{
    public static async Task Main(string[] args)
    {
       var menu = new MenuOptionsChoices();
       await menu.MenuOptions();
       Console.ReadLine();
    }
}
