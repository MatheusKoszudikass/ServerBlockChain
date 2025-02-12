using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerBlockChain.Entiites;
using ServerBlockChain.InstructionsSocket;

namespace ServerBlockChain.Conection;

public class SocketServer
{
    private static readonly List<ClientMine> _clientsConnected = [];

    public static async Task<Socket> StartAsync(uint port)
    {
        var serverSocket = new Socket(AddressFamily.InterNetwork,
         SocketType.Stream, ProtocolType.Tcp);

        var endPoint = new IPEndPoint(IPAddress.Any, checked((int)port));
        serverSocket.Bind(endPoint);
        serverSocket.Listen(100);

        Console.WriteLine($"Servidor está rodando em {endPoint}");

        Console.WriteLine("Aguardando conexão de clientes...");

        var client = await ConnectClientAsync(serverSocket);

        // InteractServer.InteractWithClientsAsync(_clientsConnected);

        await ChatMessengerServer.StartChatAsync(client, _clientsConnected);
        return serverSocket;
    }

    private static async Task<Socket> ConnectClientAsync(Socket serverSocket)
    {
        while (true)
        {
            var client = await serverSocket.AcceptAsync();

            Console.WriteLine($"Cliente conectado: {client.RemoteEndPoint}");
            lock (_clientsConnected)
            {
                var clientMine = new ClientMine
                {
                    Ip = client.RemoteEndPoint?.ToString() ?? "Unknown",
                    SO = Environment.OSVersion.VersionString,
                    HoursRunning = Environment.TickCount / 3600000
                };

                _clientsConnected.Add(clientMine);

            }
            return client;
        }
    }

    public static void StatusClientConnected()
    {
        foreach (var client in _clientsConnected)
        {
            Console.WriteLine($"Clientes conectados: {client.Ip}");
            Console.WriteLine($"Sistema Operacional: {client.SO}");
        }
    }

    public static async void DesconetClient(Socket client)
    {
        await client.DisconnectAsync(true);
        client.Close();
    }
}