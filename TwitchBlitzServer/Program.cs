using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using WebSocketSharp.Server;

class Program
{
    internal static List<Option> options = new List<Option>();
    internal static WebSocketServer wssv;
    private static int voteDuration;
    private static int numberOfOptions;

    static void Main(string[] args)
    {
        UdpServer.Start();

        LoadConfigurations();
        TwitchBot.InitializeTwitchBot();
        InitializeWebSocketServer();

        VotingManager.Initialize(voteDuration, numberOfOptions);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        wssv.Stop();
        TwitchBot.client.Disconnect();
        UdpServer.Stop();
    }

    static void LoadConfigurations()
    {
        // Load Twitch bot credentials
        try
        {
            string configText = File.ReadAllText("config.json");
            dynamic config = JsonConvert.DeserializeObject(configText);
            Config.BotUsername = config.bot_username;
            Config.AccessToken = config.access_token;
            Config.ChannelName = config.channel_name;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config.json: {ex.Message}");
            Environment.Exit(1);
        }

        // Load voting options
        try
        {
            string optionsText = File.ReadAllText("voting_options.json");
            dynamic optionsConfig = JsonConvert.DeserializeObject(optionsText);
            voteDuration = optionsConfig.Countdown;
            numberOfOptions = optionsConfig.CurrentOptions;
            foreach (var option in optionsConfig.options)
            {
                options.Add(new Option
                {
                    Name = option.Name,
                    Command = option.Command,
                    SubItemTypes = option.SubItemTypes.ToObject<List<string>>()
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading voting_options.json: {ex.Message}");
            Environment.Exit(1);
        }
    }

    static void InitializeWebSocketServer()
    {
        wssv = new WebSocketServer("ws://localhost:8080");
        wssv.AddWebSocketService<VoteService>("/vote");
        wssv.Start();
        Console.WriteLine("WebSocket server started on ws://localhost:8080");
    }
}
