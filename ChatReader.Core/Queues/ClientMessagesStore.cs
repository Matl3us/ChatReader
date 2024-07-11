using System.Collections.Concurrent;

namespace ChatReader.Core.Queues
{
    public class ClientMessagesStore
    {
        private readonly Dictionary<string, ConcurrentQueue<string>> _queuesDict = [];

        public void AddMessage(string id, string message)
        {
            _queuesDict.TryAdd(id, new ConcurrentQueue<string>());
            if (_queuesDict.TryGetValue(id, out var queue))
            {
                queue.Enqueue(message);
            }
        }

        public bool TryGetMessage(string id, out string? message)
        {
            if (_queuesDict.TryGetValue(id, out var queue))
            {
                return queue.TryDequeue(out message);
            }

            message = null;
            return false;
        }

        public bool TryGetMessageQueue(string id, out ConcurrentQueue<string>? queue)
        {
            return _queuesDict.TryGetValue(id, out queue);
        }

        public bool TryDeleteMessageQueue(string id)
        {
            return _queuesDict.Remove(id);
        }
    }
}
