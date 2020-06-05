using System;
using System.Threading.Tasks;
using WebCrawler.Models;

namespace WebCrawler
{
    public interface IWebCrawler
    {
        /// <summary>
        /// Event that calling when new url info received by crawler
        /// </summary>
        event Action<UrlInfo> UrlInfoReceived;

        /// <summary>
        /// Processes web site {url}
        /// </summary>
        /// <param name="url">root url to start crawler</param>
        /// <param name="maxThreads">maximum number of urls processing at the same time</param>
        /// <param name="processExternalUrls">if false, crawler will process only urls with root url prefix</param>
        /// <param name="contentTypesToLoad">array of content types that will loaded and analyzed to get new urls</param>
        /// <returns></returns>
        Task<SiteInfo> ProcessAsync(string url, int maxThreads, bool processExternalUrls, string[] contentTypesToLoad);
    }
}
