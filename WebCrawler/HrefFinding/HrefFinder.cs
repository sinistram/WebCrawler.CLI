using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebCrawler.HrefFinding
{
    public class HrefFinder : IHrefFinder
    {
        public IEnumerable<string> GetHrefUrls(string baseUrl, string html)
        {
            if (baseUrl is null)
            {
                throw new ArgumentNullException(baseUrl);
            }

            if (html is null)
            {
                throw new ArgumentNullException(html);
            }

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var hrefs = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
                if (hrefs != null)
                {
                    return GetUrlsFromHref(baseUrl, hrefs);
                }
                return Enumerable.Empty<string>();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        private IEnumerable<string> GetUrlsFromHref(string baseUrl, HtmlNodeCollection hrefs)
        {
            foreach (var link in hrefs)
            {
                var hrefValue = link.GetAttributeValue("href", null);
                if (hrefValue != null && !Regex.IsMatch(hrefValue, "^https?://"))
                {
                    hrefValue = new Uri(new Uri(baseUrl), hrefValue).AbsoluteUri;
                }
                yield return hrefValue;
            }
        }
    }
}
