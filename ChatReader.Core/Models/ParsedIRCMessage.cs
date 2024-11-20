using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatReader.Core.Models
{
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
    }

    public struct ParsedIRCMessage
    {
        public string Prefix { get; set; }
        public IRCMessageCommand Command { get; set; }
        public string Params { get; set; }
        public ITags Tags { get; set; }

        public string User { get; set; }
        public string Channel { get; set; }
        public string Content { get; set; }
    }

    public class TagsConverter : JsonConverter<ITags>
    {
        public override ITags Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<PRIVMSGTags>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, ITags value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
