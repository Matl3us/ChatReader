namespace ChatReader;

public static class Parser
{
    public static IRCMessage ParseMessage(string message)
    {
        message = message.TrimEnd();
        var iRCMessage = new IRCMessage();
        string[] msgFragments = message.Split(' ');

        bool tagsPresent = msgFragments[0].StartsWith('@');
        string tagsString = tagsPresent ? msgFragments[0] : "";
        if (tagsPresent)
        {
            ParseTags(tagsString, ref iRCMessage);
        }

        int prefixIndex = tagsPresent ? 1 : 0;
        bool prefixPresent = msgFragments[prefixIndex].StartsWith(':');
        string prefixString = prefixPresent ? msgFragments[prefixIndex] : "";
        if (prefixPresent)
        {
            ParsePrefix(prefixString, ref iRCMessage);
        }

        int commandIndex = (tagsPresent ? 1 : 0) + (prefixPresent ? 1 : 0);
        string command = msgFragments[commandIndex];
        string paramsString = string.Join(" ", msgFragments[(commandIndex + 1)..]);
        ParseCommand(command, paramsString, ref iRCMessage);

        return iRCMessage;
    }

    public static void ParseTags(string tagsString, ref IRCMessage iRCMessage)
    {
        var tagsList = new Dictionary<string, string>();
        string[] tags = tagsString[1..].Split(';');

        foreach (string tag in tags)
        {
            string[] parts = tag.Split('=');
            if (parts.Length == 2)
            {
                tagsList.Add(parts[0], parts[1]);
            }
        }

        Console.WriteLine("--- Tags ---");
        foreach (var tag in tagsList)
        {
            Console.WriteLine($"Key: {tag.Key} Value: {tag.Value}");
        }
    }

    public static void ParsePrefix(string prefixString, ref IRCMessage iRCMessage)
    {
        string[] prefixFragments = prefixString[1..].Split('!');
        iRCMessage.Prefix = prefixString;

        if (prefixFragments.Length == 2)
        {
            iRCMessage.User = prefixFragments[0];
        }
    }

    public static void ParseCommand(string command, string paramsString, ref IRCMessage iRCMessage)
    {
        IRCMessageCommand value;
        iRCMessage.Command = Enum.TryParse(command, out value) ? value : IRCMessageCommand.INVALID;

        if (iRCMessage.Command == IRCMessageCommand.PRIVMSG)
        {
            string[] paramsFragments = paramsString.Split(':', 2);
            iRCMessage.Channel = paramsFragments[0][1..^1];
            if (paramsFragments.Length > 1)
            {
                iRCMessage.Content = paramsFragments[1];
            }
        }
        iRCMessage.Params = paramsString;
    }
}