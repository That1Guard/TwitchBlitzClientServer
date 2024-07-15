using WebSocketSharp;
using WebSocketSharp.Server;

public class VoteService : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        Send("Connected to the vote server.");
    }
}
