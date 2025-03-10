using System.Net.Sockets;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Handler;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public sealed class ClientService : IClientService
    {
        private readonly IMenuDisplayService _menuDisplayService;
        private readonly IClientInteractionService _clientInteractionService;
        private readonly IDataMonitorService<ClientMine> _dataMonitorService;
        private readonly IClientManager _clientManager;
        private readonly IClientMonitor _clientMonitor;

        public event Action<int>? TotalClientConnected;
        private readonly HashSet<ClientMine> _clientMines = [];
        private ServerListener _serverListener = new(5000);
        private CancellationTokenSource? _cancellationTokenSource = new();
        private string[] _options = [];

        public ClientService(IMenuDisplayService menuDisplayService,
            IClientInteractionService clientInteractionService,
            IDataMonitorService<ClientMine> dataMonitorService, IClientManager clientManager,
            IClientMonitor clientMonitor)
        {
            _menuDisplayService = menuDisplayService;
            _clientInteractionService = clientInteractionService;
            _dataMonitorService = dataMonitorService;
            _clientManager = clientManager;
            _clientMonitor = clientMonitor;
            var globalEventBus = GlobalEventBus.InstanceValue;

            globalEventBus.Subscribe<ClientMine>(OnClientMineAdd);
        }

        public void Start(ServerListener serverListener)
        {
            try
            {
                this._clientInteractionService.ReturnToClientService += () => Start(_serverListener);
                if (!serverListener.Listening)
                    return;
                // _clientMines.Clear();
                // PopulateClientMines(152_246);
                _serverListener = serverListener;
                Console.Clear();
                _menuDisplayService.DeleteOption();
                _menuDisplayService.RegisterOption(0, ConnectClient);
                _menuDisplayService.RegisterOption(1, StopListening);
                _menuDisplayService.RegisterOption(2, () => GetAllConnectedClients(TypeHelp.Menu));
                _menuDisplayService.RegisterOption(3, SelectClient);
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
                _menuDisplayService.TotalClientConnected = _clientMines.Count;
                //  _= Task.Run(() => MonitorClients());
                _menuDisplayService.SelectedOption(TypeHelp.Select, this._options, this._options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error to connect client {ex.Message}");
            }
        }

        private void OnClientMineAdd(ClientMine clientMine)
        {
            _clientMines.Add(clientMine);
            _menuDisplayService.TotalClientConnected = _clientMines.Count;
            Console.WriteLine($"{clientMine.Id}");
        }

        public void PopulateClientMines(int count)
        {
            var random = new Random();
            for (var i = 1; i <= count; i++)
            {
                _clientMines.Add(new ClientMine()
                {
                    IpPublic =
                        $"{random.Next(1, 223)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(1, 256)}",
                    Name = i.ToString() + 1,
                    So = "Windows",
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

                _menuDisplayService.DisplayMenu(TypeHelp.Menu);
                Console.ReadKey();
                Start(_serverListener);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when accepting client: " + ex.Message);
            }
        }

        public void GetAllConnectedClients(TypeHelp type)
        {
            _menuDisplayService.DeleteOption();
            this._options =
            [
                .._clientMines.Select(
                    client =>
                        $"{client.IpPublic} - Name: {client.Name} SO:{client.So} - Status: {client.Status} - {DateTime.Now}")
            ];

            _menuDisplayService.SelectedView(type, this._options);

            Start(_serverListener);
        }

        public void SelectClient()
        {
            _menuDisplayService.DeleteOption();
            this._options =
            [
                .. _clientMines.Select(
                    client =>
                        $"Id:{client.Id}, - IP public: {client.IpPublic} - Name: {client.Name} SO:{client.So} - Status: {client.Status} - {DateTime.Now}")
            ];

            if (_menuDisplayService.SelectedOption(TypeHelp.Select, this._options, [.. _clientMines]) is ClientMine
                clientMine)
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
            var ipClient = socketClient.LocalEndPoint?.ToString()?.Split(":")[0];

            var clientToRemove = _clientMines.FirstOrDefault(c => c.IpLocal == ipClient || c.IpPublic == ipClient);

            if (clientToRemove != null)
            {
                _clientMines.Remove(clientToRemove);
                Console.WriteLine($"Client {clientToRemove.Name} disconnected.");
            }
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

        private void OnTotalClientConnected(int obj)
        {
            TotalClientConnected?.Invoke(obj);
        }
    }
}