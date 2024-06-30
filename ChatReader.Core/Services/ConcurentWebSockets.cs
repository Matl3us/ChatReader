using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ChatReader.Core.Services
{
    public class ConcurentWebSockets
    {
        public ConcurrentDictionary<string, WebSocket> webSocketsDict = new();
    }
}
