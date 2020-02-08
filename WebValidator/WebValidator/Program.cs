using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WebValidator.Configuration;
using WebValidator.Crawler;
using WebValidator.Json;
using WebValidator.Logger;

namespace WebValidator
{
    internal class Program
    {
        private static string _baseUrl;
        private static string _browser;
        private static int _depth;
        private static void Main()
        {
            InitConfig();
            ILogger logger = new ConsoleLogger();
            var watch = Stopwatch.StartNew();
            var crawler = new HtmlCrawler(logger, _depth, _baseUrl);
            crawler.Crawl(0,_baseUrl);
            var pages = crawler.GetPages();
            watch.Stop();

            LogCrawlErrors(pages, logger);

            logger.Log($"Found: {pages.Count}");
            logger.Log("Search time: " + watch.Elapsed);

            SaveToJson(pages);

            ////file:///D:\\Studia\\WebValidator\\Web\\Web1.html
        }

        private static void LogCrawlErrors(IReadOnlyDictionary<string, Node> pages, ILogger logger)
        {
            foreach (var (_, node) in pages)
            {
                var errors = node.GetErrors();
                foreach (var error in errors)
                {
                    logger.Log("Url: " + node.GetUrl());
                    logger.Log(error);
                }
            }
        }

        private static void InitConfig()
        {
            var config = ConfigurationLoader.GetOption<Config>();
            _baseUrl = config.Url;
            _browser = config.Browser;
            _depth = config.Depth;
        }

        private static void SaveToJson(IReadOnlyDictionary<string, Node> pages)
        {
            var nodes = pages.Values.OrderBy(p => p.GetUrl());
            var nodesDto = new VisitedPagesDto()
            {
                Pages = new List<NodeDto>()
            };

            foreach (var node in nodes)
            {
                List<string> parentNodes;
                if ((int)node.GetStatusCode() >= 200 && (int)node.GetStatusCode() <= 299)
                {
                    parentNodes = new List<string>();
                }
                else
                {
                    parentNodes = node.GetParentNodes().ToList();
                }


                nodesDto.Pages.Add(new NodeDto
                {
                    ParentNodes = parentNodes,
                    StatusCode = node.GetStatusCode(),
                    Url = node.GetUrl()
                });
            }
            

            File.WriteAllText(Directory.GetCurrentDirectory() + @"\json.json", new SaveToJson().Serialize(nodesDto));
        }

        //private static IEnumerable<string> Check(string url)
        //{
        //    _page.OpenPage(new Uri(url), 5);
        //    var urls = _page.GetAttributes("a", "href");
        //    var errors = new Validator.Validator(new Request.Request(), new ConsoleLogger()).Validate(urls);
        //    Logger.Log("Errors for " + url);
        //    Logger.LogErrors(errors);
        //    return urls;
        //}
    }
}