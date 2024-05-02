using System.Text;

namespace ChatReader
{
    enum IRCMessage
    {
        JOIN,
        NICK,
        NOTICE,
        PART,
        PASS,
        PING,
        PONG,
        PRIVMSG,
    }

    public struct ChatMessage
    {
        public string User { get; set; }
        public string Command { get; set; }
        public string Channel { get; set; }
        public string Content { get; set; }
        public override readonly string ToString()
        {
            return $"User: {User} | Command: {Command} | Channel: {Channel}\nContent: {Content}\n";
        }
    }

    public static class Parser
    {
        public static string ParseMessage(string message)
        {
            List<ChatMessage> chatMessages = [];
            string[] singleMsg = message.Trim().Split('\n');
            foreach (var msg in singleMsg)
            {
                ChatMessage chatMessage = new();
                string[] parts = msg.Split(':');
                string prefix = parts[1];

                ParsePrefix(prefix, ref chatMessage);
                if (parts.Length > 2)
                {
                    string content = parts[2];
                    chatMessage.Content = content;
                }

                chatMessages.Add(chatMessage);
            }

            StringBuilder sb = new();
            foreach (var msg in chatMessages)
            {
                sb.Append(msg.ToString());
            }
            return sb.ToString();
        }

        public static void ParsePrefix(string prefix, ref ChatMessage chatMessage)
        {
            string[] parts = prefix.Trim().Split(' ');
            foreach (var part in parts)
            {
                ParsePrefixPart(part, ref chatMessage);
            }
        }

        public static void ParsePrefixPart(string part, ref ChatMessage chatMessage)
        {
            if (part.EndsWith("tmi.twitch.tv"))
            {
                chatMessage.User = part.Contains('@') ? part.Split('!')[0] : "tmi.twitch.tv";
            }
            else if (Enum.GetNames(typeof(IRCMessage)).Contains(part))
            {
                chatMessage.Command = part;
            }
            else if (part.StartsWith('#'))
            {
                chatMessage.Channel = part;
            }
        }
    }
}