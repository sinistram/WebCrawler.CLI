using System;
using System.IO;
using WebCrawler.Models;

namespace WebCrawler.CLI
{
    internal class WebCrawlerApp : IDisposable
    {
        private readonly IWebCrawler _webCrawler;
        private readonly CommandLineOptions _options;

        public WebCrawlerApp(IWebCrawler webCrawler, CommandLineOptions options)
        {
            _webCrawler = webCrawler;
            _options = options;
            _webCrawler.UrlInfoReceived += WriteUrlInfo;
        }

        public void Process()
        {
            _webCrawler.ProcessAsync(_options.Url, _options.MaxThreads, false, new[] { "text/html" }).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _webCrawler?.Dispose();
        }

        private void WriteUrlInfo(UrlInfo urlInfo)
        {
            File.AppendAllText(_options.ResultPath, $"{urlInfo}{Environment.NewLine}");
        }
    }
}
