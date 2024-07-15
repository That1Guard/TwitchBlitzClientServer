using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

public static class VotingManager
{
    public static Dictionary<string, int> UserVotes { get; private set; } = new Dictionary<string, int>();
    public static Dictionary<int, int> Votes { get; private set; } = new Dictionary<int, int>();
    public static bool VotingActive { get; private set; } = false;
    private static Timer voteTimer;
    private static List<Option> currentOptions = new List<Option>();
    private static List<Option> previousOptions = new List<Option>();
    private static int voteDuration;
    private static int numberOfOptions;
    private static Random random = new Random();

    public static void Initialize(int duration, int optionsCount)
    {
        voteDuration = duration;
        numberOfOptions = optionsCount;
    }

    public static void StartVoting()
    {
        Votes.Clear();
        UserVotes.Clear(); // Reset user votes
        VotingActive = true;

        // Randomly select the specified number of options for this round, avoiding immediate repeats
        currentOptions = GetRandomOptions(numberOfOptions, previousOptions);

        // Store current options to avoid repeats in the next round
        previousOptions = new List<Option>(currentOptions);

        Console.WriteLine("Voting started! Options:");
        StringBuilder messageBuilder = new StringBuilder("New voting round has started!: ");
        for (int i = 0; i < currentOptions.Count; i++)
        {
            int optionNumber = i + 1; // Increment option number
            Votes[optionNumber] = 0;
            Console.WriteLine($"{optionNumber}: {currentOptions[i].Name}");
            messageBuilder.AppendLine($"{optionNumber}: {currentOptions[i].Name}");
        }

        TwitchBot.Client.SendMessage(Config.ChannelName, messageBuilder.ToString());
        voteTimer = new Timer(EndVoting, currentOptions, TimeSpan.FromSeconds(voteDuration), Timeout.InfiniteTimeSpan);
        BroadcastVoteStart(currentOptions); // Broadcast vote start
    }

    private static List<Option> GetRandomOptions(int count, List<Option> avoidOptions)
    {
        List<Option> shuffledOptions;
        do
        {
            shuffledOptions = Program.options.OrderBy(x => random.Next()).ToList();
        } while (shuffledOptions.Take(count).SequenceEqual(avoidOptions.Take(count)));

        return shuffledOptions.Take(count).ToList();
    }

    private static void EndVoting(object state)
    {
        VotingActive = false;
        voteTimer.Dispose();

        var randomOptions = (List<Option>)state;

        var winningOption = Votes.OrderByDescending(v => v.Value).FirstOrDefault();
        Console.WriteLine($"Voting ended! Winner: Option {winningOption.Key} ({randomOptions[winningOption.Key - 1].Name}) with {winningOption.Value} votes");

        var selectedOption = randomOptions[winningOption.Key - 1];

        // Store the base command
        var baseCommand = selectedOption.Command;

        // Prepare the final command to be sent
        var finalCommand = baseCommand;

        // Choose a random sub-item type if available and update the final command
        if (selectedOption.SubItemTypes != null && selectedOption.SubItemTypes.Count > 0)
        {
            var randomSubItemType = selectedOption.SubItemTypes.OrderBy(x => random.Next()).FirstOrDefault();
            finalCommand = $"{baseCommand} {randomSubItemType}";
        }

        TwitchBot.Client.SendMessage(Config.ChannelName, $"Voting ended! Winner: Option {winningOption.Key} with {winningOption.Value} votes");

        UdpServer.SendResultToClient(finalCommand);
        BroadcastVoteEnd(); // Broadcast vote end
    }

    private static void BroadcastVoteStart(List<Option> randomOptions)
    {
        var message = new
        {
            type = "voteStart",
            options = randomOptions.Select(o => o.Name).ToList(),
            duration = voteDuration
        };
        BroadcastMessage(message);
    }

    public static void BroadcastVoteUpdate()
    {
        var message = new
        {
            type = "voteUpdate",
            votes = Votes,
            optionNames = currentOptions.ToDictionary(o => (currentOptions.IndexOf(o) + 1).ToString(), o => o.Name)
        };
        BroadcastMessage(message);
    }

    private static void BroadcastVoteEnd()
    {
        var message = new
        {
            type = "voteEnd",
            votes = Votes
        };
        BroadcastMessage(message);
    }

    private static void BroadcastMessage(object message)
    {
        string jsonMessage = JsonConvert.SerializeObject(message);
        Program.wssv.WebSocketServices.Broadcast(jsonMessage);
    }
}
