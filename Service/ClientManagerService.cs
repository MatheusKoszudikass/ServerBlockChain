using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerBlockChain.Entities;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class ClientManagerService : IClientManager
    {
        private readonly Dictionary<Guid, ClientInfo> _clients = [];
        public event Action<ClientInfo>? AddedClientAct;
        public event Action<ClientInfo>? DisconnectedClientAtc;
        public event Action<ClientInfo>? RemovedClientAct;

        public void AddClient(Socket socket, SslStream sslStream)
        {
            var clientInfo = new ClientInfo
            {
                Socket = socket,
                SslStream = sslStream
            };

            _clients[clientInfo.Id] = clientInfo;
            AddedClientAct?.Invoke(clientInfo);
        }

        public void DisconnectClient(Socket socket)
        {
            var clientInfo = _clients.Values.FirstOrDefault(c => c.Socket == socket);
            if (clientInfo != null)
            {
                DisconnectClient(clientInfo);
            }
            else
            {
                Console.WriteLine("Client with given socket not found.");
            }
        }

        public void DisconnectClient(SslStream sslStream)
        {
            var clientInfo = _clients.Values.FirstOrDefault(c => c.SslStream == sslStream);
            if (clientInfo != null)
            {
                DisconnectClient(clientInfo);
            }
            else
            {
                Console.WriteLine("Client with given SslStream not found.");
            }
        }

        private void DisconnectClient(ClientInfo clientInfo)
        {
            _clients.Remove(clientInfo.Id);
            DisconnectedClientAtc?.Invoke(clientInfo);
        }

        public void RemoveClient(Guid clientId)
        {
            if (_clients.TryGetValue(clientId, out var clientInfo))
            {
                RemoveClient(clientInfo);
            }
            else
            {
                Console.WriteLine($"Client {clientId} not found.");
            }
        }

        public void RemoveClient(Socket socket)
        {
            var clientInfo = _clients.Values.FirstOrDefault(c => c.Socket == socket);
            if (clientInfo != null)
            {
                RemoveClient(clientInfo);
            }
            else
            {
                Console.WriteLine("Client with given socket not found.");
            }
        }

        public void RemoveClient(SslStream sslStream)
        {
            var clientInfo = _clients.Values.FirstOrDefault(c => c.SslStream == sslStream);
            if (clientInfo != null)
            {
                RemoveClient(clientInfo);
            }
            else
            {
                Console.WriteLine("Client with given SslStream not found.");
            }
        }

        private void RemoveClient(ClientInfo clientInfo)
        {
            _clients.Remove(clientInfo.Id);
            clientInfo.SslStream?.Close();
            clientInfo.Socket?.Close();
            DisconnectedClientAtc?.Invoke(clientInfo);
            RemovedClientAct?.Invoke(clientInfo);
        }

        public ClientInfo? GetClient(Guid clientId)
        {
            _clients.TryGetValue(clientId, out var clientInfo);
            return clientInfo;
        }

        public IEnumerable<ClientInfo> GetAllClients()
        {
            return _clients.Values;
        }

        public ClientInfo? GetLastClient()
        {
            return _clients.Values.LastOrDefault();
        }
    }
}
