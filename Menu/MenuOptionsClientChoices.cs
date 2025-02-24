using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Menu
{
    public class MenuOptionsClientChoices
    {
        public void ClientMenuOptions(ClientMine clientMines)
        {
            Console.Clear();
            Console.WriteLine($"Interacao com o cliente:{clientMines.Ip} - SO: {clientMines.SO} - Status: {clientMines.Status} - {DateTime.Now}");
            Console.WriteLine("1 - Iniciar uma conversa");
            Console.WriteLine("2 - Sair");

            var option = Console.ReadLine();

            if (int.TryParse(option, out int result))
            {
                ClientSelectedOption(result, clientMines);
            }
            else
            {
                Console.WriteLine("Opção inválida. Presseione qualquer tecla para voltar ao menu principal.");
                Console.ReadLine();
            }
        }

        private void ClientSelectedOption(int option, ClientMine clientMines)
        {
            switch (option)
            {
                case 1:
                    InitChat(clientMines);
                    break;
                case 2:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Opção inválida. Pressione qualquer tecla para continuar...");
                    Console.ReadLine();
                    break;
            }
        }

        private static void InitChat(ClientMine clientMines)
        {
            
        }

        private static void ReceiveAsync()
        {

        }

        private static void SendAsync()
        {

        }
    }
}