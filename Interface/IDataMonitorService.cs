using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Interface
{
    public interface IDataMonitorService<T>
    {
        public event Action<T> DataReceived;
        Task StartDepencenciesAsync(Socket socket, Certificate certificate);
        void StartDepencenciesAsync(Socket socket, SslStream sslStream);
        Task ReceiveDataAsync(); 
        Task SendDataAsync(T data);
        void StopMonitoring();
    }
}