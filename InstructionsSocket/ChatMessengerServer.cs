using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerBlockChain.Conection;
using ServerBlockChain.Entiites;

namespace ServerBlockChain.InstructionsSocket
{
    public class ChatMessengerServer
    {
        public static async Task StartChatAsync(Socket serverSocket, List<ClientMine> clientsConnected)
        {
            try
            {
                Task receiveTask = Task.Run(() => ListenForMessagesAsync(serverSocket, clientsConnected));

                while (serverSocket.Connected)
                {
                    await SendMessageAsync(serverSocket, clientsConnected);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no chat: {ex.Message}");
            }
            finally
            {
                // lock(clientsConnected)
                // {
                //     string? ipClient = serverSocket.RemoteEndPoint?.ToString();
                //     clientsConnected.Remove(clientsConnected.Find(client => client?.Ip == ipClient) ?? new ClientMine());
                // }

                // SocketClient.StatusClientConnected();
                // Console.WriteLine("Conexão encerrada.");
                // serverSocket.Close();
            }
        }

        private static async Task ListenForMessagesAsync(Socket serverSocket, List<ClientMine> clientsConnected)
        {
            try
            {
                while (serverSocket.Connected)
                {
                    byte[] buffer = new byte[1024];
                    Console.Write("> ");
                    int bytesRead = await serverSocket.ReceiveAsync(buffer, SocketFlags.None);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"[Client]: {message}");
                        continue;
                    }
                    break;
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Conexão com o servidor perdida.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao receber mensagem: {ex.Message}");
            }
            finally
            {
                // lock(clientsConnected)
                // {
                //     string? ipClient = serverSocket.RemoteEndPoint?.ToString();
                //     clientsConnected.Remove(clientsConnected.Find(client => client?.Ip == ipClient) ?? new ClientMine());
                // }

                // SocketClient.StatusClientConnected();
                // Console.WriteLine("Conexão encerrada.");
                // serverSocket.Close();
            }
        }

        public static async Task SendMessageAsync(Socket serverSocket, List<ClientMine> clientsConnected)
        {
            try
            {
                string? message = await Task.Run(() => Console.ReadLine());
                Console.Write("> ");
                if (!string.IsNullOrWhiteSpace(message))
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    await serverSocket.SendAsync(messageBytes, SocketFlags.None);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Erro na conexão com o servidor.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem: {ex.Message}");
            }
            finally
            {
                // lock(clientsConnected)
                // {
                //     string? ipClient = serverSocket.RemoteEndPoint?.ToString();
                //     clientsConnected.Remove(clientsConnected.Find(client => client?.Ip == ipClient) ?? new ClientMine());
                // }

                // SocketClient.StatusClientConnected();
                // Console.WriteLine("Conexão encerrada.");
                // serverSocket.Close();
            }
        }
    }
}
