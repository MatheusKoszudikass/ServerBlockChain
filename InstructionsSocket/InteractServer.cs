using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Conection;
using ServerBlockChain.Entiites;

namespace ServerBlockChain.InstructionsSocket
{
    public class InteractServer
    {

        public static void InteractWithClientsAsync(List<ClientMine> clientConntected)
        {
            MenuLayout(clientConntected);
            // while (clientConntected.Count > 0)
            // {
            //     if (SelectClient()?.ToLower() == "exit") break;

            //     if (int.TryParse(SelectClient(), out int clientIndex)
            //     && clientIndex >= 0 && clientIndex < clientConntected.Count)
            //     {
            //         SelectedClientAsync(clientConntected, clientIndex);
            //     }
            // }
        }

        public static void MenuLayout(List<ClientMine> clientsConnected)
        {
            int selectedIndex = 0;
            ConsoleKey key;

            try
            {
                do
                {
                    Console.Clear();
                    Console.WriteLine("Escolha um cliente para interagir:");

                    DisplayClient(clientsConnected, selectedIndex);

                    if (Console.KeyAvailable)
                    {
                        break;
                    }
                    key = Console.ReadKey(true).Key;
                    selectedIndex = SelectArrowDisplay(key, clientsConnected, selectedIndex);

                } while (key != ConsoleKey.Enter);

                Console.Clear();
                var selectedClient = clientsConnected[selectedIndex];
                Console.WriteLine($"Cliente Selecionado: \nIP: {selectedClient.Ip} \nSistema Operacional: {selectedClient.SO}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao interagir com o cliente: {ex.Message}");
            }

        }
        private static void DisplayClient(List<ClientMine> clientsConnected, int selectedIndex)
        {
            for (int i = 0; i < clientsConnected.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {i + 1}. {clientsConnected[i].Ip} | SO: {clientsConnected[i].SO}");
                    Console.ResetColor();
                    continue;
                }

                Console.WriteLine($"{i + 1}. IP: {clientsConnected[i].Ip} | SO: {clientsConnected[i].SO}");
            }
        }

        private static int SelectArrowDisplay(ConsoleKey key, List<ClientMine> clientsConnected, int selectedIndex)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? clientsConnected.Count - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == clientsConnected.Count - 1) ? 0 : selectedIndex + 1;
                    break;
            }
            return selectedIndex;
        }

        private static string? SelectClient()
        {
            Console.WriteLine("\nEscolha um cliente para interagir (digite o número ou 'exit' para sair):");
            SocketClient.StatusClientConnected();

            string? input = Task.Run(() => Console.ReadLine()).Result;
            return input;
        }

        private static void SelectedClientAsync(List<ClientMine> clientConntected, int clientIndex)
        {
            var selectedClient = clientConntected[clientIndex];

            Console.WriteLine($"Cliente selecionado: {selectedClient}");
        }
    }
}