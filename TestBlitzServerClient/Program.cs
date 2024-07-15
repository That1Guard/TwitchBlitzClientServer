using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TestClient
{
    static void Main(string[] args)
    {
        UdpClient udpClient = new UdpClient();
        udpClient.Connect("127.0.0.1", 11000);

        string message = "Hello Server";
        byte[] data = Encoding.ASCII.GetBytes(message);
        udpClient.Send(data, data.Length);

        var remoteEP = new IPEndPoint(IPAddress.Any, 0);
        var receivedData = udpClient.Receive(ref remoteEP);
        string receivedMessage = Encoding.ASCII.GetString(receivedData);

        Console.WriteLine("Received from server: " + receivedMessage);
    }
}
