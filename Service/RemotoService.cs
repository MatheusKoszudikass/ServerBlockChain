using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class RemotoService (IMenuDisplayService menuDisplayService, 
    IClientService clientService) : IRemotoService
    {
        private readonly IMenuDisplayService _menuDisplayService = menuDisplayService;
        private readonly IClientService _clientService = clientService;
        private ServerListener? _workSocket;

        public void CreateRemoto()
        {
            try
            {
                _menuDisplayService.RegisterOption(0, () => StartRemoto());
                _menuDisplayService.RegisterOption(1, StopRemoto);

                string[] options = 
                [ 
                    "Interact with the server",
                    "Exit"
                ];
                _menuDisplayService.SelectedOption(TypeHelp.Select, options, options);
            }
            catch (SocketException ex)
            {
                throw new Exception($"Error when creating the remote server. {ex.Message}");
            }
        }

        private void StartRemoto()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var cts = cancellationTokenSource.Token;
     
                string[] options = ["Enter the port to start the server or press ENTER to use port 5000:"];

                _menuDisplayService.DisplayMenu(TypeHelp.Menu, options);

                var port = _menuDisplayService.SelectedIndex();

                if (port == 0) port = 5000;

                _workSocket = new ServerListener(checked((uint)port));

                _workSocket.DisconnectClientAct += (SocketClient) => {
                    _clientService.DisconnectClient(SocketClient);
                };

                _workSocket.Start();
    
                _= _workSocket.StartListeningForClients(cts);

                _clientService.Start(_workSocket);
            }
            catch (SocketException ex)
            {
                throw new Exception($"Error when starting the remote server. {ex.Message}");
            }
        }

        public void StopRemoto()
        {
            try
            {
                _workSocket?.Stop();
                Console.WriteLine("Stop remote server.");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when stopping the remote server: {ex.Message}");
            }
        }
    }
}