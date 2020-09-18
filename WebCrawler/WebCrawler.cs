using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.HrefFinding;
using WebCrawler.Models;
using WebCrawler.Requesting;

namespace WebCrawler
{
    public class WebCrawler : IWebCrawler
    {
        private readonly SemaphoreSlim _siteProcessingSemaphore;
        private readonly IWebPageRequester _webPageRequester;
        private readonly IHrefFinder _hrefFinder;
        private readonly ILogger<WebCrawler> _logger;

        private string[] _contentTypesToLoad;
        private SemaphoreSlim _urlProcessingSemaphore;
        private CrawlerQueue _crawlerQueue;
        private SiteInfo _siteInfo;
        private bool _processExternalUrls;
        private bool _processing;

        private List<Task> _urlProcessingTasks;

        public event Action<UrlInfo> UrlInfoReceived;

        public WebCrawler(IWebPageRequester webPageRequester, IHrefFinder hrefFinder, ILogger<WebCrawler> logger)
        {
            _hrefFinder = hrefFinder;
            _webPageRequester = webPageRequester;
            _siteProcessingSemaphore = new SemaphoreSlim(1, 1);
            _logger = logger;
        }

        public async Task<SiteInfo> ProcessAsync(string url, int maxThreads, bool processExternalUrls, string[] contentTypesToLoad)
        {
            return await ProcessAsync(url, maxThreads, processExternalUrls, contentTypesToLoad, CancellationToken.None);
        }

        public async Task<SiteInfo> ProcessAsync(string url, int maxThreads, bool processExternalUrls, string[] contentTypesToLoad, CancellationToken token)
        {
            _logger.LogInformation("Waiting for _siteProcessingSemaphore. (Only one processing can be executed for the one instance at the same time)");
            await _siteProcessingSemaphore.WaitAsync(token).ConfigureAwait(false);

            _logger.LogInformation($"Start processing url {url}");
            try
            {
                if (url is null)
                {
                    throw new ArgumentNullException(nameof(url));
                }

                if (maxThreads < 1)
                {
                    throw new ArgumentException($"{nameof(maxThreads)} should be greater than zero.");
                }
                _processing = true;

                _processExternalUrls = processExternalUrls;
                _contentTypesToLoad = contentTypesToLoad;
                _urlProcessingSemaphore = new SemaphoreSlim(maxThreads, maxThreads);
                _crawlerQueue = new CrawlerQueue();
                _siteInfo = new SiteInfo(url);
                _urlProcessingTasks = new List<Task>();
                _crawlerQueue.EnqueueIfNotProcessed(url);

                await ProcessSiteAsync(token).ConfigureAwait(false);

                return _siteInfo;
            }
            finally
            {
                _processing = false;
                _logger.LogInformation("Release _siteProcessingSemaphore");
                _siteProcessingSemaphore.Release();
            }
        }

        public void Dispose()
        {
            _siteProcessingSemaphore?.Dispose();
            _urlProcessingSemaphore?.Dispose();
        }

        private async Task ProcessSiteAsync(CancellationToken token)
        {
            while(_processing)
            {
                if (token.IsCancellationRequested)
                {
                    _processing = false;
                    break;
                }

                await _urlProcessingSemaphore.WaitAsync(token);
                Task task = null;
                try
                {
                    if (_crawlerQueue.TryDequeue(out var url))
                    {
                        task = ProcessUrl(url);
                        _urlProcessingTasks.Add(task);
                    }
                    else
                    {
                        if (_urlProcessingTasks.All(t => t.IsCompleted))
                        {
                            if (_crawlerQueue.IsEmpty)
                            {
                                _logger.LogInformation("All tasks are finished and there is no item in queue. Processing finished.");
                                _processing = false;
                            }
                        }
                        else
                        {
                            await Task.Delay(2000, token).ConfigureAwait(false);
                        }
                    }
                }
                finally
                {
                    _ = task?.ContinueWith((t) => _urlProcessingSemaphore.Release(), token);
                }
            }
        }

        private async Task ProcessUrl(string url)
        {
            _logger.LogInformation($"Processing {url} started.");
            var webPageContent = await _webPageRequester.GetWebPageContent(url, _contentTypesToLoad).ConfigureAwait(false);

            var urlInfo = new UrlInfo
            {
                Url = webPageContent.Url,
                Length = webPageContent.Length,
                ContentType = webPageContent.ContentType
            };

            _logger.LogInformation($"New urlInfo: {urlInfo}");
            _siteInfo.UrlInfos.Add(urlInfo);
            UrlInfoReceived?.Invoke(urlInfo);

            if (webPageContent.Content != null)
            {
                var urls = _hrefFinder.GetHrefUrls(url, webPageContent.Content).ToArray();
                foreach (var newUrl in urls)
                {
                    if (_processExternalUrls || url.StartsWith(_siteInfo.RootUrl)) 
                    {
                        _crawlerQueue.EnqueueIfNotProcessed(newUrl);
                    }
                }
            }

            _logger.LogInformation($"Processing {url} finished.");
        }
    }
}
