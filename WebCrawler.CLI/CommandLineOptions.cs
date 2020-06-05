using CommandLine;

namespace WebCrawler.CLI
{
    internal class CommandLineOptions
    {
        [Option('u', Required = true, HelpText = "Set url to the site.")]
        public string Url { get; set; }

        [Option('m', Required = true, HelpText = "Set max threads count.")]
        public int MaxThreads { get; set; }

        [Option('r', Required = true, HelpText = "Set result file path.")]
        public string ResultPath { get; set; }
    }
}
