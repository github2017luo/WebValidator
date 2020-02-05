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
            var pages = crawler.GetVisitedPages();
            watch.Stop();

            foreach (var error in crawler.GetErrors())
            {
                logger.Log($"{error.Key} {error.Value}");
            }

            logger.Log($"Found: {pages.Count}");
            logger.Log("Search time: " + watch.Elapsed);

            SaveToJson(pages);


            ////file:///D:\\Studia\\WebValidator\\Web\\Web1.html
        }

        private static void InitConfig()
        {
            var config = ConfigurationLoader.GetOption<Config>();
            _baseUrl = config.Url;
            _browser = config.Browser;
            _depth = config.Depth;
        }

        private static void SaveToJson(IReadOnlyDictionary<string, bool> pages)
        {
            var keys = pages.Keys.ToList();
            keys.Sort();

            var dto = new VisitedPagesDto
            {
                Pages = keys
            };

            File.WriteAllText(Directory.GetCurrentDirectory() + @"\json.json", new SaveToJson().Serialize(dto));
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