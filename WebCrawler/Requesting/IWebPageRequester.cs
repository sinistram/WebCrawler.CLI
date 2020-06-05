using JetBrains.Annotations;
using System.Threading.Tasks;

namespace WebCrawler.Requesting
{
    public interface IWebPageRequester
    {
        Task<WebPageContent> GetWebPageContent([NotNull] string url, [CanBeNull] string[] contentTypesToLoad);
    }
}
