using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WebValidator.Crawler;
using WebValidator.Json;
using WebValidator.Logger;

namespace WebValidator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var depth = GetDepth(args[2]);
            ILogger logger = new ConsoleLogger();
            var watch = Stopwatch.StartNew();
            var crawler = new HtmlCrawler(logger, depth, args[1]);
            crawler.Crawl(0, args[1]);
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

        private static int GetDepth(string depth)
        {
            return Int32.Parse(depth);
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