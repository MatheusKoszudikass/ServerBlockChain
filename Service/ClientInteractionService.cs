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
        private ClientMine _clientMine = new();
        private string[] _options = [];

        public void Start(ClientMine clientMine)
        {
            try
            {
                if (clientMine == null) ReturnToClientService?.Invoke();

                _clientMine = clientMine ?? throw new ArgumentNullException(nameof(clientMine));

                _menuDisplayService.DeleteOption();
                _menuDisplayService.RegisterOption(0, () => ShowClientInfo());
                // _menuDisplayService.RegisterOption(1, () => SendChatMessage(""));
                // _menuDisplayService.RegisterOption(2, () => SendFile("C:\\Users\\User\\Desktop\\file.txt"));
                // _menuDisplayService.RegisterOption(3, () => SendObject(new object()));
                // _menuDisplayService.RegisterOption(4, () => ReturnToClientService?.Invoke());

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
                    $"Client SO: {_clientMine.So}",
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

        public Task SendChatMessageAsync(string message)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveChatMessageAsync()
        {
            throw new NotImplementedException();
        }

        public Task SendFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveFileAsync(string destinationPath)
        {
            throw new NotImplementedException();
        }

        public Task SendObjectAsync<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> ReceiveObjectAsync<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }
}