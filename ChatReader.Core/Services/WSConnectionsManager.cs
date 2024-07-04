using ChatReader.Core.Models;
using ChatReader.Core.Services;
using ChatReader.Core.Utils;
using System.Net.WebSockets;

namespace ChatReader.Core;

public class WSConnectionsManager(WebSocketConnections webSocketConnections, TwitchIRCConnections twitchIRCConnections)
{
    private readonly WebSocketConnections _clientConnections = webSocketConnections;
    private readonly TwitchIRCConnections _twitchServerConnections = twitchIRCConnections;

    public async Task StartConnectionAsync(WebSocket webSocket, AuthUserDto authUser)
    {
        var twitchConnection = _twitchServerConnections.ConnectionsDict.GetOrAdd(authUser.Id, new TwitchIRCConnection());
        var clientConnection = _clientConnections.WebSocketsDict.GetOrAdd(authUser.Id, new WebSocketConnection(webSocket));

        await TwitchIRCHandler.StartConnectionAsync(twitchConnection, CancellationToken.None);

        var tasks = new[]
        {
            WebSocketHandler.SendMessagesAsync(webSocket, twitchConnection.MessageQueue, CancellationToken.None),
            WebSocketHandler.ReceiveMessagesAsync(webSocket, clientConnection.MessageQueue, CancellationToken.None),
            TwitchIRCHandler.SendMessagesAsync(twitchConnection, clientConnection.MessageQueue, CancellationToken.None),
            TwitchIRCHandler.ReceiveMessagesAsync(twitchConnection, authUser, CancellationToken.None)
        };
        await Task.WhenAny(tasks);
    }

    public bool TryAdd(string id, WebSocket webSocket)
    {
        var clientConnection = new WebSocketConnection(webSocket);
        return _clientConnections.WebSocketsDict.TryAdd(id, clientConnection);
    }

    public bool TryRemove(string id)
    {
        return _clientConnections.WebSocketsDict.TryRemove(id, out var _)
            && _twitchServerConnections.ConnectionsDict.TryRemove(id, out var _);
    }
}
