using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WebCrawler.HrefFinding;
using WebCrawler.Requesting;

namespace WebCrawler.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(RunApp)
                .WithNotParsed(PrintErrors);
        }

        private static void RunApp(CommandLineOptions options)
        {
            try
            {
                var services = ConfigureServices(options);
                var provider = services.BuildServiceProvider();
                using (var app = provider.GetService<WebCrawlerApp>())
                {
                    app.Process();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static IServiceCollection ConfigureServices(CommandLineOptions options)
        {
            var serviceCollection = new ServiceCollection();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            serviceCollection.AddTransient<IHrefFinder, HrefFinder>();
            serviceCollection.AddTransient<IWebPageRequester, WebPageRequester>();
            serviceCollection.AddTransient<IWebCrawler, WebCrawler>();
            serviceCollection.AddTransient<WebCrawlerApp>();

            serviceCollection.AddTransient(p => options);

            return serviceCollection;
        }

        private static void PrintErrors(IEnumerable<Error> errors)
        {
            Console.WriteLine("Error: command-line arguments were not parsed correctly");
            foreach (var error in errors)
            {
                try
                {
                    var namedError = error as NamedError;
                    Console.WriteLine($"Not parsed: {namedError?.NameInfo.LongName}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected exception when not found an argument: {e}");
                }
            }
        } 
    }
}
