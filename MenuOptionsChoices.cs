using ServerBlockChain.Conection;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Menu;

namespace ServerBlockChain
{
    public class MenuOptionsChoices
    {
        public async Task MenuOptions()
        {
            Console.Clear();
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1 - Iniciar servidor");
            Console.WriteLine("2 - Sair");

            var option = Console.ReadLine();

            if (int.TryParse(option, out int result))
            {
                SelectedOption(result);
                return;
            }

            await MenuOptions();
        }

        private async void SelectedOption(int option)
        {
            switch (option)
            {
                case 1:
                    await ServerStart();
                    break;
                case 2:
                    Console.WriteLine("Saindo...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    await MenuOptions();
                    break;
            }
        }

        private static async Task ServerStart()
        {
            Console.Clear();
            uint defaultPort = 5000;
            Console.Write("Digite a porta para iniciar o servidor ou pressione Enter para usar a padrão (5000):");

            string input = Console.ReadLine() ?? string.Empty;
            uint port = defaultPort;
            if (!string.IsNullOrEmpty(input) && (!uint.TryParse(input, out port) || port <= 0))
            {
                Console.WriteLine("Por favor, digite um número válido para a porta (deve ser maior que 0):");
                await ServerStart();
                return;
            }

            var result = SocketServer.StartAsync(port);
            if (result.Listening){
                ConsoleHelp.WriteSuccess(TypeHelp.Success, $"Servidor iniciado com sucesso! Porta:{port}");
                var menuServer = new MenuOptionsServerChoices();
                menuServer.ServerMenuOption(result);
            } 
        }
    }
}