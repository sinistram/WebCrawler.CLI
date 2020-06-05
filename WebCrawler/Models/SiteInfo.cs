using System.Collections.Concurrent;

namespace WebCrawler.Models
{
    public class SiteInfo
    {
        public string RootUrl { get; }

        public ConcurrentBag<UrlInfo> UrlInfos { get; }

        public SiteInfo(string rootUrl)
        {
            RootUrl = rootUrl;
            UrlInfos = new ConcurrentBag<UrlInfo>();
        }
    }
}
