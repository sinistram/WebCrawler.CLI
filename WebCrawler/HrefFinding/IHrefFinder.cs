using JetBrains.Annotations;
using System.Collections.Generic;

namespace WebCrawler.HrefFinding
{
    public interface IHrefFinder
    {
        [NotNull]
        IEnumerable<string> GetHrefUrls([NotNull] string baseUrl, [NotNull] string html);
    }
}
