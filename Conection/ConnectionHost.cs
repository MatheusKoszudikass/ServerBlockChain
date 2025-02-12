using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServerBlockChain.Connection;
public class ConnectionHost
{
    private uint Port { get; set; } = 0;
    private string IpLocal { get; set; } = "";
    private string IPublic { get; set; } = "";

    private static readonly HttpClient httpClient = new();

    public async Task InitializeConnection()
    {

        this.IpLocal = GetLocalIpAddress();
        Console.WriteLine($"IP local: {this.IpLocal}");

        this.IPublic = await GetPublicIPAdress();
        Console.WriteLine($"IP público: {this.IPublic}");
    }

    public string GetConnectionHostIpLocal()
    {
        return $"{this.IpLocal}:{this.Port}";
    }

    public string GetConnectionHostIpPublic()
    {
        return $"{this.IPublic}:{this.Port}";
    }

    public static string GetLocalIpAddress()
    {
        string localIp = "127.0.0.1";

        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                foreach (var ipAddress in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ipAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIp = ipAddress.Address.ToString();
                        break;
                    }
                }
            }
        }

        return localIp;
    }

    public static async Task<string> GetPublicIPAdress()
    {
        try
        {
            return await httpClient.GetStringAsync("https://api.ipify.org");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter IP público: {ex.Message}");
            return "Erro";
        }
    }
}

