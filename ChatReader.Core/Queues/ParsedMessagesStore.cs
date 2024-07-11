using ChatReader.Core.Models;
using System.Collections.Concurrent;

namespace ChatReader.Core.Queues
{
    public class ParsedMessagesStore
    {
        private readonly Dictionary<string, ConcurrentQueue<ParsedIRCMessage>> _queuesDict = [];

        public void AddParsedMessage(string id, ParsedIRCMessage message)
        {
            _queuesDict.TryAdd(id, new ConcurrentQueue<ParsedIRCMessage>());
            if (_queuesDict.TryGetValue(id, out var queue))
            {
                queue.Enqueue(message);
            }
        }

        public bool TryGetParsedMessage(string id, out ParsedIRCMessage message)
        {
            if (_queuesDict.TryGetValue(id, out var queue))
            {
                return queue.TryDequeue(out message);
            }

            message = new ParsedIRCMessage();
            return false;
        }

        public bool TryGetMessageQueue(string id, out ConcurrentQueue<ParsedIRCMessage>? queue)
        {
            return _queuesDict.TryGetValue(id, out queue);
        }

        public bool TryDeleteMessageQueue(string id)
        {
            return _queuesDict.Remove(id);
        }
    }
}
