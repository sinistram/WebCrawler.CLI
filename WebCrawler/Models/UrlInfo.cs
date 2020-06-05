namespace WebCrawler.Models
{
    public class UrlInfo
    {
        public string Url { get; set; }
        public long? Length { get; set; }
        public string ContentType { get; set; }

        public override string ToString()
        {
            return $"{Url}: {ContentType}, {Length}";
        }
    }
}
