namespace ChatReader.Utils;

public static class Parser
{
    public static IRCMessage ParseMessage(string message)
    {
        message = message.TrimEnd();
        var iRCMessage = new IRCMessage();
        string[] msgFragments = message.Split(' ');

        bool tagsPresent = msgFragments[0].StartsWith('@');
        string tagsString = tagsPresent ? msgFragments[0] : "";

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
        if (tagsPresent)
        {
            ParseTags(tagsString, ref iRCMessage);
        }

        return iRCMessage;
    }

    public static void ParseTags(string tagsString, ref IRCMessage iRCMessage)
    {
        string[] tagsFragments = tagsString[1..].Split(';');
        ITags tags;
        switch (iRCMessage.Command)
        {
            case IRCMessageCommand.PRIVMSG:
                tags = ParsePRIVMSGTags(tagsFragments);
                break;
            default:
                return;
        }
        iRCMessage.Tags = tags;
    }

    public static ITags ParsePRIVMSGTags(string[] tagsFragments)
    {
        var tags = new PRIVMSGTags();
        foreach (string tag in tagsFragments)
        {
            string[] parts = tag.Split('=');
            if (parts.Length == 2)
            {
                switch (parts[0])
                {
                    case "badge-info":
                        tags.BadgeInfo = parts[1];
                        break;
                    case "badges":
                        string[] badges = parts[1].Split(',');
                        tags.Badges = new List<string>(badges);
                        break;
                    case "bits":
                        if (int.TryParse(parts[1], out int bits))
                        {
                            tags.Bits = bits;
                        }
                        break;
                    case "color":
                        tags.Color = parts[1];
                        break;
                    case "display-name":
                        tags.DisplayName = parts[1];
                        break;
                    case "emotes":
                        string[] emotes = parts[1].Split(',');
                        tags.Emotes = new List<string>(emotes);
                        break;
                    case "id":
                        tags.Id = parts[1];
                        break;
                    case "mod":
                        tags.Mod = parts[1] != "0";
                        break;
                    case "room-id":
                        tags.RoomId = parts[1];
                        break;
                    case "subscriber":
                        tags.Subscriber = parts[1] != "0";
                        break;
                    case "tmi-sent-ts":
                        if (double.TryParse(parts[1], out double time))
                        {
                            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                            dateTime = dateTime.AddMilliseconds(time).ToLocalTime();
                            tags.TmiSentTs = dateTime;
                        }
                        break;
                    case "turbo":
                        tags.Turbo = parts[1] != "0";
                        break;
                    case "user-id":
                        tags.UserId = parts[1];
                        break;
                    case "user-type":
                        tags.UserType = parts[1];
                        break;
                    case "vip":
                        tags.Vip = parts[1] != "0";
                        break;
                }
            }
        }
        return tags;
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