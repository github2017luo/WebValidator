using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WebValidator.Configuration;
using WebValidator.Crawler;
using WebValidator.Json;
using WebValidator.Logger;
using WebValidator.Request;
using WebValidator.SeleniumDriver;

namespace WebValidator
{
    internal class Program
    {
        private static string _baseUrl;
        private static string _url;
        private static string _browser;
        private static int _depth;
        private static ICrawler _crawler;
        private static ILogger _logger;

        private static void Main()
        {
            InitConfig();
            _logger = new ConsoleLogger();
            InitCrawler();

            var watch = Stopwatch.StartNew();

            //var crawler = new HtmlCrawler(_logger, _depth, _baseUrl);
            _crawler.Crawl(0, _url);

            var pages = _crawler.GetPages();
            new Validator.Validator(new RestClient()).Validate(pages.Values);

            watch.Stop();

            LogCrawlErrors(pages, _logger);

            _logger.Log($"Found: {pages.Count}");
            _logger.Log("Search time: " + watch.Elapsed);

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

        private static void InitCrawler()
        {
            if (String.IsNullOrEmpty(_browser))
            {
                _crawler = new HtmlCrawler(_logger, _depth, _baseUrl);
                return;
            }

            _crawler = new SeleniumCrawler(_browser, _logger, new SeleniumDriverInitilizer(), _depth, _baseUrl); 
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
            

            File.WriteAllText(Directory.GetCurrentDirectory() + @"\Results.json", new SaveToJson().Serialize(nodesDto));
        }
    }
}