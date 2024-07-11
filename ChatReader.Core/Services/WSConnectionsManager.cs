using ChatReader.Core.Models;
using ChatReader.Core.Models.Dto;
using ChatReader.Core.Queues;
using ChatReader.Core.Utils;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ChatReader.Core;

public class WSConnectionsManager(ParsedMessagesStore parsedMessagesStore, ClientMessagesStore clientMessagesStore)
{
    private readonly ParsedMessagesStore _parsedMessagesStore = parsedMessagesStore;
    private readonly ClientMessagesStore _clientMessagesStore = clientMessagesStore;

    private ConcurrentQueue<ParsedIRCMessage> _parsedMsgQueue = new();
    private ConcurrentQueue<string> _clientMsgQueue = new();

    public bool TryCreateConnection(AuthUserDto authUserDto)
    {
        var id = authUserDto.Id;
        if (string.IsNullOrEmpty(id)) { return false; }
        bool isSuccessful = _parsedMessagesStore.TryGetMessageQueue(id, out var parsedQueue);
        isSuccessful &= _clientMessagesStore.TryGetMessageQueue(id, out var clientQueue);

        if (!isSuccessful) { return false; }

        _parsedMsgQueue = parsedQueue ?? new ConcurrentQueue<ParsedIRCMessage>();
        _clientMsgQueue = clientQueue ?? new ConcurrentQueue<string>();

        return isSuccessful;
    }

    public async Task StartConnectionAsync(WebSocket webSocket, AuthUserDto authUser)
    {
        var ircClient = await IRCHandler.StartConnectionAsync(CancellationToken.None);

        var tasks = new[]
        {
            WebSocketHandler.SendMessagesAsync(webSocket, _parsedMsgQueue, CancellationToken.None),
            WebSocketHandler.ReceiveMessagesAsync(webSocket, _clientMsgQueue, CancellationToken.None),
            IRCHandler.SendMessagesAsync(ircClient, _clientMsgQueue, CancellationToken.None),
            IRCHandler.ReceiveMessagesAsync(ircClient, _parsedMsgQueue ,authUser, CancellationToken.None)
        };
        await Task.WhenAny(tasks);
    }
}
