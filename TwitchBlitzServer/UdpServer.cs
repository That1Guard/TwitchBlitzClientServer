using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public static class UdpServer
{
    private static UdpClient udpServer;
    private static IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
    private static bool isRunning = true;
    private static Thread udpThread;

    static UdpServer()
    {
        try
        {
            udpServer = new UdpClient(11000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing UdpServer: {ex.Message}");
            throw;
        }
    }

    public static void Start()
    {
        Console.WriteLine("UDP server started. Waiting for commands...");
        udpThread = new Thread(new ThreadStart(ReceiveUdpCommands));
        udpThread.Start();
    }

    private static void ReceiveUdpCommands()
    {
        while (isRunning)
        {
            try
            {
                if (udpServer.Available > 0)
                {
                    var data = udpServer.Receive(ref remoteEP);
                    string command = Encoding.ASCII.GetString(data).Trim();
                    Console.WriteLine($"Received command: {command}");

                    if (command.StartsWith("!vstart"))
                    {
                        VotingManager.StartVoting();
                    }
                    else if (command.Equals("!exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Stop();
                    }
                }
                else
                {
                    Thread.Sleep(100); // Add a small delay to avoid busy waiting
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving UDP command: {ex.Message}");
            }
        }
    }

    public static void Stop()
    {
        Console.WriteLine("Stopping UDP server...");
        isRunning = false;
        udpServer.Close();
        udpThread.Join(); // Wait for the thread to finish
        Console.WriteLine("UDP server stopped.");
    }

    public static void SendResultToClient(string result)
    {
        try
        {
            var response = Encoding.ASCII.GetBytes(result);
            udpServer.Send(response, response.Length, remoteEP);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending result to client: {ex.Message}");
        }
    }
}