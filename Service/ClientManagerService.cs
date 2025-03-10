using System.Net.Security;
using System.Net.Sockets;
using ServerBlockChain.Entities;
using ServerBlockChain.Handler;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class ClientManagerService : IClientManager
    {
        private readonly Dictionary<Guid, ClientInfo> _clients = [];
        private readonly GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;

        public void AddClient(Socket socket, SslStream sslStream)
        {
            var clientInfo = new ClientInfo
            {
                Socket = socket,
                SslStream = sslStream
            };

            _clients[clientInfo.Id] = clientInfo;
            _globalEventBus.Publish(clientInfo);
        }


        public void UpdateClientInfoComplete(Guid clientId, bool status)
        {
            var clientInfo = _clients.Values.FirstOrDefault(c => c.Id == clientId);
            if (clientInfo != null)
            {
                clientInfo.StatusInfoComplete = status;
            }
        }

        public void DisconnectClient(Socket socket)
        {
            var clientInfo = _clients.Values.FirstOrDefault(c => c.Socket == socket);
            if (clientInfo != null)
            {
                DisconnectClient(clientInfo);
            }
        }

        public void DisconnectClient(SslStream sslStream)
        {
            var clientInfo = _clients.Values.FirstOrDefault(c => c.SslStream == sslStream);
            if (clientInfo != null)
            {
                DisconnectClient(clientInfo);
            }
        }

        private void DisconnectClient(ClientInfo clientInfo)
        {
            _clients.Remove(clientInfo.Id);
            _globalEventBus.Publish(clientInfo);
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
            _globalEventBus.Publish(clientInfo);

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

        public IEnumerable<ClientInfo> GetAllClientsNoCompleteInfo()
        {
           return _clients.Values.Where(c => !c.StatusInfoComplete);
        }

        public ClientInfo? GetLastClient()
        {
            return _clients.Values.LastOrDefault();
        }
    }
}