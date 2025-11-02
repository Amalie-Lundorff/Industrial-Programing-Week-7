using System.Net.Sockets;
using System.Text;

namespace InventorySystems_Week6.Models;

public class Robot
{
    public const int urscriptPort = 30002, dashboardPort = 29999;
    public string IpAddress = "localhost";

    public void SendString(int port, string message)
    {
        using var client = new TcpClient(IpAddress, port);
        using var stream = client.GetStream();
        stream.Write(Encoding.ASCII.GetBytes(message));
    }

    public void SendUrscript(string urscript)
    {
        // Frigør bremse og send programmet
        SendString(dashboardPort, "brake release\n");
        SendString(urscriptPort, urscript);
    }
}