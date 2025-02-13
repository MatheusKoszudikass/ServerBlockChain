using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Menu
{
    public class MenuOptionsServerChoices
    {
        private readonly List<ClientMine> _clientMines = new();
        private readonly List<Socket> _clientsConnected = new();

        public void ServerMenuOption(Listener listener)
        {
            if (!listener.Listening) return;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Servidor iniciado. Escolha uma opção:");
                Console.WriteLine("1 - Ver todos os clientes conectados");
                Console.WriteLine("2 - Atualizar clientes em tempo real");
                Console.WriteLine("3 - Selecionar cliente");
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

        private void ServerSelectedOption(int option, Listener listener)
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
                Console.WriteLine($"Cliente conectado: {_clientMines[i].Ip} - SO: {_clientMines[i].SO} - {DateTime.Now}");
            }

            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadLine();
        }

        private void MonitorClients(Listener listener)
        {
            try
            {
                Console.Clear();
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                 _ = Task.Run(async () =>
                {
                    while (!cancellationToken.IsCancellationRequested && listener.Listening)
                    {
                        var workSocket = await listener.AcceptClientAsync();
                        if (workSocket != null)
                        {
                            var clientMine = new ClientMine
                            {
                                Ip = workSocket.RemoteEndPoint?.ToString() ?? "Unknown",
                                SO = Environment.OSVersion.VersionString,
                                HoursRunning = Environment.TickCount / 3600000
                            };

                            lock (_clientsConnected)
                            {
                                _clientsConnected.Add(workSocket);
                                _clientMines.Add(clientMine);
                            }

                            Console.WriteLine($"> Novo cliente conectado: {clientMine.Ip}");
                        }
                    }
                }, cancellationToken);

                Console.WriteLine("Monitorando clientes conectados. Pressione qualquer tecla para voltar ao menu...");
                Console.ReadLine();
                cancellationTokenSource.Cancel();

                Console.Clear();
                Console.WriteLine("Voltando ao menu principal...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao monitorar clientes {ex.Message}");
                throw;
            }

        }


        // private async Task UpdateClients(Listener listener)
        // {
        //     while (listener.Listening)
        //     {
        //         var workSocket = await listener.AcceptClientAsync();

        //         if (workSocket != null)
        //         {
        //             var clientMine = new ClientMine
        //             {
        //                 Ip = workSocket.RemoteEndPoint?.ToString() ?? "Unknown",
        //                 SO = Environment.OSVersion.VersionString,
        //                 HoursRunning = Environment.TickCount / 3600000
        //             };

        //             lock (_clientsConnected)
        //             {
        //                 _clientsConnected.Add(workSocket);
        //                 _clientMines.Add(clientMine);
        //             }

        //             Console.WriteLine($"Novo cliente conectado: {clientMine.Ip}");
        //             Console.WriteLine("Pressione qualquer tecla para continuar...");
        //             Console.ReadLine();
        //         }
        //     }
        // }

        private void SelectClient()
        {
            Console.Clear();
            Console.WriteLine("Seleção de cliente não implementada ainda.");
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadLine();
        }
    }
}
