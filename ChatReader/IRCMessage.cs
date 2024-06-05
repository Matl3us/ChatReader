namespace ChatReader;

public enum IRCMessageCommand
{
    JOIN,               // Send
    NICK,               // Send
    NOTICE,             // Receive
    PART,               // Send, Receive
    PASS,               // Send
    PING,               // Receive
    PONG,               // Send
    PRIVMSG,            // Send, Receive
    CLEARCHAT,          // Receive
    CLEARMSG,           // Receive
    GLOBALUSERSTATE,    // Receive
    HOSTTARGET,         // Receive
    RECONNECT,          // Receive
    ROOMSTATE,          // Receive
    USERNOTICE,         // Receive
    USERSTATE,          // Receive
    WHISPER,            // Receive
    INVALID
}

public struct IRCMessage
{
    public string Prefix { get; set; }
    public IRCMessageCommand Command { get; set; }
    public string Params { get; set; }

    public string User { get; set; }
    public string Channel { get; set; }
    public string Content { get; set; }

    public override readonly string ToString()
    {
        if (Command == IRCMessageCommand.PRIVMSG)
        {
            return $"[{Channel}] {User}: {Content}";
        }
        return $"Prefix {Prefix} | Command {Command} | Params {Params}";
    }
}