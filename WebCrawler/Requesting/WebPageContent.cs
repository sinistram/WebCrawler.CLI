namespace WebCrawler.Requesting
{
    public class WebPageContent
    {
        public string Url { get; set; }
        public string Content { get; set; }
        public long? Length { get; set; }
        public string ContentType { get; set; }
    }
}
