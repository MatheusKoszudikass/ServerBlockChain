using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading.Tasks;
using ServerBlockChain.Conection.Client;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class ClientService(IMenuDisplayService menuDisplayService,
     IClientInteractionService clientInteractionService) : IClientService
    {
        private readonly IMenuDisplayService _menuDisplayService = menuDisplayService;
        private readonly IClientInteractionService _clientInteractionService = clientInteractionService;
        private readonly HashSet<ClientMine> _clientMines = [];
        private ServerListener _serverListener = new(5000);
        private CancellationTokenSource? _cancellationTokenSource;
        private string[] _options = [];

        public void Start(ServerListener serverListener)
        {
            try
            {
                this._clientInteractionService.ReturnToClientService += () => Start(_serverListener);
                if (!serverListener.Listening)
                    return;
                // _clientMines.Clear();
                // PopulateClientMines(152_000);
                _serverListener = serverListener;
                Console.Clear();
                _menuDisplayService.DeleteOption();
                _menuDisplayService.RegisterOption(0, () => ConnectClient());
                _menuDisplayService.RegisterOption(1, () => StopListening());
                _menuDisplayService.RegisterOption(2, () => GetAllConnectedClients(TypeHelp.Menu));
                _menuDisplayService.RegisterOption(3, () => SelectClient());
                // _menuDisplayService.RegisterOption(4, () => DisconnectClient());
                _menuDisplayService.RegisterOption(5, () => Environment.Exit(0));


                this._options =
                [
                    "Enable clients connection",
                    "Disable clients connection",
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

        public void PopulateClientMines(int count)
        {
            var random = new Random();
            for (int i = 1; i <= count; i++)
            {
                _clientMines.Add(new ClientMine(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    IpPublic = $"{random.Next(1, 223)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(1, 256)}",
                    Name = i.ToString() + 1,
                    SO = "Windows",
                    HoursRunning = i * 10
                });
            }
        }

        public void ConnectClient()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;
                Console.Clear();

                this._options =
                [
                    $"A new connection with a client..."
                ];

                _menuDisplayService.DisplayMenu(TypeHelp.Select, this._options, -1);
                _ = ListenerConnectionClient(cancellationToken);

                _menuDisplayService.DisplayMenu(TypeHelp.Menu);
                Console.ReadKey();
                Start(_serverListener);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when accepting client: " + ex.Message);
            }
        }

        private async Task ListenerConnectionClient(CancellationToken cancellationToken)
        {
            try
            {
                // var list = _serverListener.GetConnectedClients();

                // for (int i = 0; i < list.Count; i++)
                // {
                //     var client = new ClientMine(list[i]);

                //     client.InfoClientMine += (client) => AddClientMine(client);

                //     _ = client.ReceiveData(cancellationToken);

                //     Console.WriteLine($"{_clientMines.Count}");
                // }

                await Task.Delay(-1, cancellationToken);
            }

            catch (Exception) when (cancellationToken.IsCancellationRequested)
            {

            }
            catch (Exception ex)
            {
                throw new Exception("Error in client listener: " + ex.Message);
            }
        }

        private void AddClientMine(ClientMine clientMine)
        {
            if (!_clientMines.Any(c => c.IpPublic == clientMine.IpPublic))
            {
                _clientMines.Add(clientMine);

                this._options =
                    [
                        $"New client connected: {clientMine.Name}"
                    ];

                _menuDisplayService.DisplayMenu(TypeHelp.Success, _options, -1);
            }
        }

        public void GetAllConnectedClients(TypeHelp type)
        {
            _menuDisplayService.DeleteOption();
            this._options = [.._clientMines.Select(
                    client => $"{client.IpPublic} - Name: {client.Name} SO:{client.SO} - Status: {client.Status} - {DateTime.Now}")];

            _menuDisplayService.SelectedView(type, this._options);

            Start(_serverListener);
        }

        public void SelectClient()
        {
            _menuDisplayService.DeleteOption();
            this._options = [.. _clientMines.Select(
                client => $"Id:{client.Id}, - IP public: {client.IpPublic} - Name: {client.Name} SO:{client.SO} - Status: {client.Status} - {DateTime.Now}")];
            var clientMine = _menuDisplayService.SelectedOption(TypeHelp.Select, this._options, [.. _clientMines]) as ClientMine;

            if (clientMine != null)
            {
                _clientInteractionService.Start(clientMine);
                return;
            }

            Console.Clear();
            Console.WriteLine("Empty client");
            _menuDisplayService.DisplayMenu(TypeHelp.Menu);
            Console.ReadKey();
            Start(_serverListener);
        }

        public void ShowClientInfo()
        {
            throw new NotImplementedException();
        }

        public void DisconnectClient(Socket socketClient)
        {

            // var ipClient = socketClient.RemoteEndPoint?.ToString()?.Split(":")[0]
            //  ?? socketClient.LocalEndPoint?.ToString()?.Split(":")[0];

            var ipClient = socketClient.LocalEndPoint?.ToString()?.Split(":")[0];

            var clientToRemove = _clientMines.FirstOrDefault(c => c.IpLocal == ipClient || c.IpPublic == ipClient);

            if (clientToRemove != null)
            {
                _clientMines.Remove(clientToRemove);
                Console.WriteLine($"Client {clientToRemove.Name} disconnected.");
            }
            // Environment.Exit(0);
        }

        public void StopListening()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            Start(_serverListener);
        }
    }
}