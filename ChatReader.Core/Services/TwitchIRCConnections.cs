using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ChatReader.Core.Services
{
    public class TwitchIRCConnections
    {
        public ConcurrentDictionary<string, TwitchIRCConnection> ConnectionsDict = new();
    }

    public class TwitchIRCConnection
    {
        public ClientWebSocket ClientWebSocket = new();
        public ConcurrentQueue<string> MessageQueue = new();
    }
}