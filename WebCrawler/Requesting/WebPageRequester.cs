using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebCrawler.Requesting
{
    public class WebPageRequester : IWebPageRequester
    {
        private readonly ILogger<WebPageRequester> _logger;

        public WebPageRequester(ILogger<WebPageRequester> logger)
        {
            _logger = logger;
        }
        public async Task<WebPageContent> GetWebPageContent(string url, string[] contentTypesToLoad)
        {
            if (url is null)
            {
                throw new ArgumentNullException(url);
            }

            var webPageContent = new WebPageContent { Url = url };
            try
            {
                using (var httpClient = new HttpClient())
                {

                    var httpResponseMessage = await httpClient.GetAsync(url).ConfigureAwait(false);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        webPageContent.ContentType = httpResponseMessage.Content.Headers.ContentType.MediaType;
                        webPageContent.Length = httpResponseMessage.Content.Headers.ContentLength;

                        if (contentTypesToLoad != null && contentTypesToLoad.Any(contentType => contentType.Equals(webPageContent.ContentType, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            webPageContent.Content = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Cannot request url: {url}");
            }

            return webPageContent;
        }
    }
}
