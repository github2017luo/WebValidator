using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WebValidator.Json;
using WebValidator.Logger;
using WebValidator.Page;
using WebValidator.SeleniumDriver;

namespace WebValidator
{
    internal class Program
    {
        private static string _browser;
        private static IPage _page;
        private static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();
            var search = new Bfs(1, "https://wsti.pl");
            search.Search(0, new ConsoleLogger(), "https://wsti.pl");
            var pages = search.GetVisitedPages();
            watch.Stop();
            foreach (var error in search.GetErrors())
            {
                Console.WriteLine($"{error.Key} {error.Value}");
            }
            Console.WriteLine($"Found: {pages.Count}");
            Console.WriteLine(watch.Elapsed);

            var k = pages.Keys.ToList();
            k.Sort();

            var dto = new VisitedPagesDto
            {
                Pages = k
            };

            File.WriteAllText(Directory.GetCurrentDirectory() + @"\json.txt", new SaveToJson().Serialize(dto));


            ////file:///D:\\Studia\\WebValidator\\Web\\Web1.html
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
