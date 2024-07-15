using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

public static class TwitchBot
{
    internal static TwitchClient client;

    public static TwitchClient Client => client;

    public static void InitializeTwitchBot()
    {
        ConnectionCredentials credentials = new ConnectionCredentials(Config.BotUsername, Config.AccessToken);
        client = new TwitchClient();
        client.Initialize(credentials, Config.ChannelName);

        client.OnLog += Client_OnLog;
        client.OnMessageReceived += Client_OnMessageReceived;
        client.OnConnected += Client_OnConnected;

        client.Connect();
    }

    private static void Client_OnLog(object sender, OnLogArgs e)
    {
        //Console.WriteLine($"{e.DateTime}: {e.BotUsername} - {e.Data}");
    }

    private static void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        if (!VotingManager.VotingActive) return;

        string userName = e.ChatMessage.Username.ToLower();
        string message = e.ChatMessage.Message.ToLower().Trim();
        int vote;

        // Check if the user has already voted in this round
        if (VotingManager.UserVotes.ContainsKey(userName))
        {
            return;
        }

        if (int.TryParse(message, out vote) && vote >= 1 && vote <= 3)
        {
            VotingManager.UserVotes[userName] = vote; // Store user's vote
            VotingManager.Votes[vote]++;
            Console.WriteLine($"Received vote for option {vote} from user {userName}");
            VotingManager.BroadcastVoteUpdate(); // Broadcast vote update
        }
    }

    private static void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        Console.WriteLine($"Connected to Twitch as {e.BotUsername}");
    }
}
