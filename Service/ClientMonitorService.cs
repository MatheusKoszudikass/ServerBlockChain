using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Entities;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class ClientMonitorService(
        IDataMonitorService<object> dataMonitorService,
        IClientManager clientManager) : IClientMonitor
    {
        private IDataMonitorService<object> _dataMonitorService = dataMonitorService;
        private IClientManager _clientManager = clientManager;
        public event Action<ClientInfo>? ClientDesconnectedAct;

        public async Task MonitorConnectionClient(Socket socket)
        {
            var clientTasks = new List<Task>();

            var clients = _clientManager.GetAllClients();

            foreach (var clientInfo in clients)
            {
                while (socket.Connected)
                {
                    Console.WriteLine($"Client {clientInfo?.Socket!.Connected} connected.");
                    _dataMonitorService.StartDepencenciesAsync(clientInfo!.Socket!, clientInfo?.SslStream!);
                    await _dataMonitorService.ReceiveDataAsync();
                    await _dataMonitorService.SendDataAsync(SendMessageDefault.MessageSuccess);
                    await Task.Delay(5000);
                }
            }

            await Task.WhenAll(clientTasks);
        }
    }
}