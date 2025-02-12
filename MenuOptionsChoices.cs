using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerBlockChain.Conection;

namespace ServerBlockChain
{
    public class MenuOptionsChoices
    {
        public static async Task<int> MenuOptions()
        {
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1 - Iniciar servidor");
            Console.WriteLine("2 - Conectar ao servidor");
            Console.WriteLine("3 - Sair");

            var option = Console.ReadLine();

            if (int.TryParse(option, out int result))
            {
                return result;
            }

            return await MenuOptions();
        }

        public static void SelectedOption(int option)
        {
            switch (option)
            {
                case 1:
                    // await ServerStart();
                    break;
                case 2:
                    Console.WriteLine("Conectando ao servidor...");
                    break;
                case 3:
                    Console.WriteLine("Saindo...");
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }

        public static void ServerStart()
        {
            // uint port = 5000;
            // Console.Write("Digite a porta para iniciar o servidor ou pressione Enter para usar a padrão (5000):");

            // if(port == 5000) SocketClient.StartAsync(port);
            
            // if (!uint.TryParse(Console.ReadLine(), out port) || port < 1)
            // {
            //    Console.WriteLine("Por favor, digite um número válido para a porta (deve ser maior que 0):");
            //    await ServerStart();
            //    return;
            // }
            // await SocketClient.StartAsync(port);
        }
    }
}