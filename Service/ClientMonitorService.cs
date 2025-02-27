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
            while (socket.Connected)
            {
                var clientInfo = _clientManager.GetLastClient();
                Console.WriteLine($"Client {clientInfo?.Socket!.Connected} connected.");
                _dataMonitorService.StartDepencenciesAsync(clientInfo!.Socket!, clientInfo?.SslStream!);
                await _dataMonitorService.ReceiveDataAsync();
                await _dataMonitorService.SendDataAsync(SendMessageDefault.MessageSuccess);
                await Task.Delay(5000);
            }
        }
    }
}