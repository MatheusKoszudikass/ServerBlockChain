using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;

namespace ServerBlockChain.Menu
{
    public class MenuOptionsServerChoices
    {
        private readonly List<ClientMine> _clientMines = [];
        private readonly List<Socket> _clientsConnected = [];

        public void ServerMenuOption(ServerListener listener)
        {
            if (!listener.Listening) return;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Servidor iniciado. Escolha uma opção:");
                Console.WriteLine("1 - Ver todos os clientes conectados");
                Console.WriteLine("2 - Atualizar clientes em tempo real");
                Console.WriteLine("3 - Selecionar um cliente");
                Console.WriteLine("4 - Sair");

                var option = Console.ReadLine();

                if (int.TryParse(option, out int result))
                {
                    ServerSelectedOption(result, listener);
                }
                else
                {
                    Console.WriteLine("Opção inválida. Presseione qualquer tecla para voltar ao menu principal.");
                    Console.ReadLine();
                }
            }
        }

        private void ServerSelectedOption(int option, ServerListener listener)
        {
            switch (option)
            {
                case 1:
                    ShowClientInfo();
                    break;
                case 2:
                    MonitorClients(listener);
                    break;
                case 3:
                    SelectClient();
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Opção inválida. Pressione qualquer tecla para continuar...");
                    Console.ReadLine();
                    break;
            }
        }

        private void ShowClientInfo()
        {
            Console.Clear();
            Console.WriteLine($"Total de clientes conectados: {_clientMines.Count}");

            for (int i = 0; i < _clientMines.Count; i++)
            {
                ConsoleHelp.WriteSuccess(TypeHelp.Success, $"Cliente conectado: {_clientMines[i].Ip} - SO: {_clientMines[i].SO} - Status: {_clientMines[i].Status} - {DateTime.Now}");
            }

            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadLine();
        }

        private Task MonitorClients(ServerListener listener)
        {
            try
            {
                Console.Clear();
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                var result = Task.Run(async () =>
               {
                   while (!cancellationToken.IsCancellationRequested && listener.Listening)
                   {
                       var workSocket = await listener.AcceptClientAsync();
                       if (workSocket != null)
                       {
                           var clientMine = new ClientMine(workSocket)
                           {
                               Ip = workSocket.RemoteEndPoint?.ToString() ?? "Unknown",
                               Status = true,
                               SO = workSocket.Handle.ToString(),
                               HoursRunning = Environment.TickCount / 3600000
                           };

                           lock (_clientsConnected)
                           {
                               _clientsConnected.Add(workSocket);
                               _clientMines.Add(clientMine);
                           }

                           ConsoleHelp.WriteSuccess(TypeHelp.Success, $"Novo cliente conectado: {clientMine.Ip}");
                       }
                   }
                   Console.WriteLine($"Total de clientes conectados: {_clientMines.Count}");
               }, cancellationToken);

                Console.WriteLine("Monitorando clientes conectados. Pressione qualquer tecla para voltar ao menu...");
                Console.ReadLine();
                cancellationTokenSource.Cancel();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao monitorar clientes {ex.Message}");
                throw;
            }

        }

        private void SelectClient()
        {
            int selectedIndex = 0;
            ConsoleKey key;

            try
            {
                do
                {
                    Console.Clear();
                    Console.WriteLine("Escolha um cliente:");

                    DisplayClient(selectedIndex);

                    if (Console.KeyAvailable)
                    {
                        break;
                    }
                    
                    key = Console.ReadKey(true).Key;

                    selectedIndex = SelectArrowDisplay(key, selectedIndex);

                } while (key != ConsoleKey.Enter);

                var selectedClient = _clientMines[selectedIndex];

                Console.WriteLine($"Cliente selecionado: {selectedClient.Ip}");

                var menuClient = new MenuOptionsClientChoices();
                menuClient.ClientMenuOptions(selectedClient);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DisplayClient(int selectedIndex)
        {
            for (int i = 0; i < _clientsConnected.Count; i++)
            {
                if (i == selectedIndex)
                {
                    ConsoleHelp.WriteSuccess(TypeHelp.Success, $"{i + 1}. {_clientsConnected[i].RemoteEndPoint}");
                    Console.ResetColor();
                    continue;
                }

                Console.WriteLine($"{i + 1}. {_clientsConnected[i].RemoteEndPoint}");
            }
        }

        private int SelectArrowDisplay(ConsoleKey key, int selectedIndex)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? _clientMines.Count - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == _clientMines.Count - 1) ? 0 : selectedIndex + 1;
                    break;
            }

            return selectedIndex;
        }
    }
}
