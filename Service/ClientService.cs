using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Conection.Client;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class ClientService(IMenuDisplayService menuDisplayService) : IClientService
    {
        private readonly IMenuDisplayService _menuDisplayService = menuDisplayService;
        private readonly List<ClientMine> _clientMines = [];
        private CancellationTokenSource? _cancellationTokenSource;
        private string[] _options = [];

        public void Start(ServerListener serverListener)
        {
            try
            {
                if (!serverListener.Listening)
                    return;

                _menuDisplayService.DeleteOption();
                _menuDisplayService.RegisterOption(0, () => ConnectClient(serverListener));
                _menuDisplayService.RegisterOption(1, () => GetAllConnectedClients(TypeHelp.Menu, serverListener));
                _menuDisplayService.RegisterOption(2, () => SelectClient());
                _menuDisplayService.RegisterOption(3, () => DisconnectClient());

                this._options =
                [
                    "Enable clients connection",
                    "Display all connected clients",
                    "Select a client",
                    "Disconnect a client",
                    "Exit"

                ];

                _menuDisplayService.SelectedOption(TypeHelp.Select, this._options, this._options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error to connect client {ex.Message}");
            }
        }


        public void ConnectClient(ServerListener serverListener)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;
                Console.Clear();

                this._options =
                [
                    $"A new connection with a client..."
                ];
                _menuDisplayService.DisplayMenu(TypeHelp.Select, this._options, -1);
                var task = ListenerConnectionClient(serverListener, cancellationToken);


                _menuDisplayService.DisplayMenu(TypeHelp.Menu);
                Console.ReadKey();
                StopListening();
                _menuDisplayService.DeleteOption();
                Start(serverListener);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when accepting client: " + ex.Message);
            }
        }

        private Task ListenerConnectionClient(ServerListener serverListener, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested && serverListener.Listening)
                    {
                        var workSocket = await serverListener.AcceptClientAsync();

                        if (cancellationToken.IsCancellationRequested)
                            return;

                        var client = new ClientMine(workSocket)
                        {
                            Ip = workSocket.RemoteEndPoint?.ToString() ?? "Unknown",
                            Status = true,
                            SO = workSocket.Handle.ToString(),
                            HoursRunning = Environment.TickCount / 3600000
                        };

                        lock (_clientMines)
                        {
                            _clientMines.Add(client);
                        }

                        _options =
                        [
                           $"New client connected: {client.Ip}"
                        ];

                        _menuDisplayService.DisplayMenu(TypeHelp.Success, _options, -1);
                    }
                }
                catch (Exception) when (cancellationToken.IsCancellationRequested)
                {

                }
                catch (Exception ex)
                {
                    throw new Exception("Error in client listener: " + ex.Message);
                }
            }, cancellationToken);
        }


        public void GetAllConnectedClients(TypeHelp type, ServerListener serverListener)
        {
            _menuDisplayService.DeleteOption();
            List<string> list;
            lock (_clientMines)
            {
                list = [.._clientMines.Select(
                    client => $"{client.Ip} - SO: {client.SO} - Status: {client.Status} - {DateTime.Now}")];

            }
            this._options = [.. list];

             _menuDisplayService.SelectedView(type, this._options);
             Start(serverListener);
        }

        public void SelectClient()
        {
            _menuDisplayService.DisplayMenu(TypeHelp.Menu);

            lock (_clientMines)
            {
                this._options = [.._clientMines.Select(
                    client => $"{client.Ip} - SO: {client.SO} - Status: {client.Status} - {DateTime.Now}")];
            }

          var index = _menuDisplayService.SelectedOption(TypeHelp.Select, this._options, this._options);

          Console.WriteLine($"{index}");
        }

        public void ShowClientInfo()
        {
            throw new NotImplementedException();
        }

        public void DisconnectClient()
        {
            throw new NotImplementedException();
        }

        public void StopListening()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
    }
}