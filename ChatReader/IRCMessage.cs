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

public interface ITags { }

public struct PRIVMSGTags : ITags
{
    public PRIVMSGTags() { }

    public string BadgeInfo { get; set; } = "";
    public List<string> Badges { get; set; } = [];
    public int Bits { get; set; } = 0;
    public string Color { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public List<string> Emotes { get; set; } = [];
    public string Id { get; set; } = "";
    public bool Mod { get; set; } = false;
    public string RoomId { get; set; } = "";
    public bool Subscriber { get; set; } = false;
    public DateTime TmiSentTs { get; set; } = new DateTime();
    public bool Turbo { get; set; } = false;
    public string UserId { get; set; } = "";
    public string UserType { get; set; } = "";
    public bool Vip { get; set; } = false;

    public override readonly string ToString()
    {
        return $"BadgeInfo: {BadgeInfo} Badges: {Badges.Count} Bits: {Bits} Color: {Color} Display Name: {DisplayName} Emotes: {Emotes.Count} Id: {Id} Mod: {Mod} Room Id: {RoomId} Subscriber: {Subscriber} Tmi Sent Ts: {TmiSentTs} Turbo: {Turbo} User Id: {UserId} User Type: {UserType} Vip: {Vip}";
    }
}

public struct IRCMessage
{
    public string Prefix { get; set; }
    public IRCMessageCommand Command { get; set; }
    public string Params { get; set; }
    public ITags Tags { get; set; }

    public string User { get; set; }
    public string Channel { get; set; }
    public string Content { get; set; }

    public override readonly string ToString()
    {
        if (Command == IRCMessageCommand.PRIVMSG)
        {
            return $"{Tags}\n[{Channel}] {User}: {Content}";
        }
        return $"Prefix {Prefix} | Command {Command} | Params {Params}";
    }
}