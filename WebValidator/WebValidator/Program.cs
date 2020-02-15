using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebValidator.Configuration;
using WebValidator.Crawler;
using WebValidator.Json;
using WebValidator.Logger;
using WebValidator.Request;

namespace WebValidator
{
    internal class Program
    {
        private static string _baseUrl;
        private static string _url;
        private static string _browser;
        private static int _depth;
        private static void Main()
        {
            InitConfig();
            ILogger logger = new ConsoleLogger();
            var watch = Stopwatch.StartNew();

            var crawler = new HtmlCrawler(logger, _depth, _baseUrl);
            crawler.Crawl(0, _url);

            var pages = crawler.GetPages();
            new Validator.Validator(new RestClient()).Validate(pages.Values);

            watch.Stop();

            LogCrawlErrors(pages, logger);

            logger.Log($"Found: {pages.Count}");
            logger.Log("Search time: " + watch.Elapsed);

            SaveToJson(pages);
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
            _url = config.Url;
            _browser = config.Browser;
            _depth = config.Depth;
            _baseUrl = new Uri(_url).GetLeftPart(UriPartial.Authority);
        }

        private static void SaveToJson(IReadOnlyDictionary<string, Node> pages)
        {
            var nodes = pages.Values;
            var nodesDto = new VisitedPagesDto
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
                    Url = node.GetUrl(),
                    Errors = node.GetErrors().ToList(),
                    Visited = node.GetVisited()
                });

                nodesDto.Pages = nodesDto.Pages.OrderBy(p => p.StatusCode).ToList();
            }
            

            File.WriteAllText(Directory.GetCurrentDirectory() + @"\json.json", new SaveToJson().Serialize(nodesDto));
        }
    }
}