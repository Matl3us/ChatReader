using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ChatReader.Core.Services
{
    public class WebSocketConnections
    {
        public ConcurrentDictionary<string, WebSocketConnection> WebSocketsDict = new();
    }

    public class WebSocketConnection(WebSocket webSocket)
    {
        public WebSocket WebSocket = webSocket;
        public ConcurrentQueue<string> MessageQueue = new();
    }
}
