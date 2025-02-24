using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using ServerBlockChain.Entities;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class ClientInteractionService(IMenuDisplayService menuDisplayService) : IClientInteractionService
    {
        public event Action? ReturnToClientService;
        private readonly IMenuDisplayService _menuDisplayService = menuDisplayService;
        private ClientMine _clientMine = new(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
        private Chat _chat = new(new ClientMine(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)));
        private string[] _options = [];

        public void Start(ClientMine clientMine)
        {
            try
            {
                if (clientMine == null) ReturnToClientService?.Invoke();

                _clientMine = clientMine ?? throw new ArgumentNullException(nameof(clientMine));

                _menuDisplayService.DeleteOption();
                _menuDisplayService.RegisterOption(0, () => ShowClientInfo());
                _menuDisplayService.RegisterOption(1, () => SendChatMessage(""));
                _menuDisplayService.RegisterOption(2, () => SendFile("C:\\Users\\User\\Desktop\\file.txt"));
                _menuDisplayService.RegisterOption(3, () => SendObject(new object()));
                _menuDisplayService.RegisterOption(4, () => ReturnToClientService?.Invoke());

                this._options =
                [
                    $"Client information",
                    $"Open chat: {_clientMine.IpPublic}",
                    $"Send file: {_clientMine.IpPublic}",
                    $"Send object: {_clientMine.Name}",
                    $"return to main menu"
                ];

                _menuDisplayService.SelectedOption(TypeHelp.Select, this._options, this._options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error to start client interaction {ex.Message}");
            }
        }

        public void ShowClientInfo()
        {
            try
            {
                this._options = [];
                this._options =
                [
                    $"Client ID: {_clientMine.Id}",
                    $"Client IP public: {_clientMine.IpPublic}",
                    $"Client IP local: {_clientMine.IpLocal}",
                    $"Client Name: {_clientMine.Name}",
                    $"Client status: {_clientMine.Status}",
                    $"Client SO: {_clientMine.SO}",
                    $"Client Hours Running: {_clientMine.HoursRunning}",
                    $"Client memory: {_clientMine.HardwareInfo?.TotalRAM}",
                    $"Client Name Processor: {_clientMine.HardwareInfo?.ProcessorName}",
                    $"Client CPU: {_clientMine.HardwareInfo?.CpuUsage}",
                    $"Client disk: {_clientMine.HardwareInfo?.DiskUsage}",

                    // $"Socket client connected: {_clientMine.SocketClient.Connected}",
                    // $"Socket client identitfied operation system: {_clientMine.SocketClient.Handle}",
                    // $"Socket client type protocol: {_clientMine.SocketClient.ProtocolType}"
                ];

                _menuDisplayService.SelectedView(TypeHelp.Menu, this._options);
                _menuDisplayService.DisplayMenu(TypeHelp.Menu);
                Start(_clientMine);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error showing client information.{ex.Message}");
            }
        }

        public void SendChatMessage(string message)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception("Error sending chat message: " + ex.Message);
            }
        }

        public void ReceiveChatMessage()
        {
            try
            {
                Console.Clear();
                this._chat = new Chat(_clientMine);
                _chat.Start();
                _chat.InfoClientMine += (client, clientMine) =>
                {
                    Console.WriteLine($"{clientMine.IpPublic} - SO: {clientMine.SO} - Status: {clientMine.Status} - {DateTime.Now}");
                    _clientMine = clientMine;
                };

                _menuDisplayService.DisplayMenu(TypeHelp.Menu);
                Console.ReadKey();
                Start(_clientMine);
            }
            catch (Exception ex)
            {
                throw new Exception("Error receiving chat message: " + ex.Message);
            }
        }

        public void SendFile(string filePath)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception("Error sending file: " + ex.Message);
            }
        }

        public void ReceiveFile(string destinationPath)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception("Error receiving file: " + ex.Message);
            }
        }

        public void SendObject(object obj)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception("Error sending object: " + ex.Message);
            }
        }

        public object ReceiveObject()
        {
            try
            {
                return new object();
            }
            catch (Exception ex)
            {
                throw new Exception("Error receiving object: " + ex.Message);
            }
        }
    }
}