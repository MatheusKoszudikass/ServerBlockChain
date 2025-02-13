using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerBlockChain.Entiites;
using ServerBlockChain.InstructionsSocket;
using System.Diagnostics;
using System.Threading.Tasks;
#pragma warning restore format

namespace ServerBlockChain.Conection;

public class SocketServer
{
    private static readonly List<ClientMine> _clientMines = [];
    private static readonly List<Socket> _clientsConnected = [];

    public static async Task StartAsync(uint port)
    {
        var listener = new Listener(port);
        listener.Start();
        Console.WriteLine("Servidor iniciado.");
        while (listener.Listening)
        {
            var clientSocket = await listener.AcceptClientAsync();
            if (clientSocket != null)
            {
                AddSocketAcceptedClient(clientSocket);
            }
        }
    }

    private static ClientMine? AddSocketAcceptedClient(Socket? listener)
    {

        if (listener == null) return null;

        var clientMine = new ClientMine
        {
            Ip = listener.RemoteEndPoint?.ToString() ?? "Unknown",
            SO = Environment.OSVersion.VersionString,
            HoursRunning = Environment.TickCount / 3600000
        };
        lock (_clientsConnected)
        {

            _clientsConnected.Add(listener);
            _clientMines.Add(clientMine);

        }
        Console.Clear();
        Console.WriteLine($"Total de clientes conectados: {_clientMines.Count}");

        for (int i = 0; i < _clientMines.Count; i++)
        {
            Console.WriteLine($"Cliente conectado: {_clientMines[i].Ip} - SO: {_clientMines[i].SO} - {DateTime.Now}");
        }
        return clientMine;
    }

    private static void RemoveSocketAccepted(Object? sender, SocketClosedEventHandler e)
    {
        lock (_clientsConnected)
        {
            _clientsConnected.Remove(e.ClosedSocket);
        }
    }

    private static async Task<Socket> ConnectClientAsync(Socket serverSocket)
    {
        while (true)
        {
            string threadId = Environment.CurrentManagedThreadId.ToString();
            var client = await serverSocket.AcceptAsync();
            Console.WriteLine($"Cliente conectado: {client.RemoteEndPoint}, thread: {threadId}");
            lock (_clientsConnected)
            {
                var clientMine = new ClientMine
                {
                    Ip = client.RemoteEndPoint?.ToString() ?? "Unknown",
                    SO = Environment.OSVersion.VersionString,
                    HoursRunning = Environment.TickCount / 3600000
                };

                _clientMines.Add(clientMine);
            }
            return client;
        }
    }

    public static void StatusClientConnected()
    {
        foreach (var client in _clientMines)
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