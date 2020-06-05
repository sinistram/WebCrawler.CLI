using System.Collections.Concurrent;
using System.Linq;

namespace WebCrawler
{
    public class CrawlerQueue
    {
        private ConcurrentQueue<string> _urlsQueue = new ConcurrentQueue<string>();
        private ConcurrentBag<string> _processedQueues = new ConcurrentBag<string>();

        public bool IsEmpty => _urlsQueue.IsEmpty;

        public void EnqueueIfNotProcessed(string url)
        {
            if (!_processedQueues.Contains(url))
            {
                _urlsQueue.Enqueue(url);
                _processedQueues.Add(url);
            }
        }

        public bool TryDequeue(out string url)
        {
            return _urlsQueue.TryDequeue(out url);
        }
    }
}