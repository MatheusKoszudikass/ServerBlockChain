using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerBlockChain.Entities;
using ServerBlockChain.InstructionsSocket;
using System.Diagnostics;
using System.Threading.Tasks;
#pragma warning restore format

namespace ServerBlockChain.Conection;

public class SocketServer
{
    private static readonly List<ClientMine> _clientMines = [];
    private static readonly List<Socket> _clientsConnected = [];

    public static ServerListener StartAsync(uint port)
    {
        var listener = new ServerListener(port);
        listener.Start();

        return listener;
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