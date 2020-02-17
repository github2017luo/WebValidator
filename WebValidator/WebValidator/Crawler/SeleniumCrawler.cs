using OpenQA.Selenium;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebValidator.Logger;
using WebValidator.Search;
using WebValidator.SeleniumDriver;
using WebValidator.Util;

namespace WebValidator.Crawler
{
    public class SeleniumCrawler : ICrawler
    {
        private readonly ISearchElements _searchElements;
        private readonly ILogger _logger;
        private readonly IWebDriver _webDriver;
        private static int? _depth;
        private static string _baseUrl;
        private static ConcurrentDictionary<string, Node> _pages;

        public SeleniumCrawler(string browser, ILogger logger, IDriverInitializer driverInitializer, int depth, string baseUrl)
        {
            _webDriver = driverInitializer.InitializeDriver(browser);
            _searchElements = new SeleniumSearchElements(_webDriver);
            _logger = logger;
            _depth ??= depth;
            _baseUrl = baseUrl;
            _pages ??= new ConcurrentDictionary<string, Node>();
        }

        public void Dispose()
        {
            _logger.Log("Dispose of browser.");
            _webDriver?.Dispose();
        }

        public void Crawl(int depth, string url)
        {
            if (depth > _depth) return;
            var node = GetNode(url);
            OpenPage(node);

            node.MakeVisited();
            _pages[url] = node;
            
            var urls = GetAttributes("a", "href").ToList();
            urls = Sanitizer.SanitizeUrls(urls, _baseUrl, _pages).ToList();

            var urlsToDelete = AddParentNode(url, urls);
            urls = urls.Except(urlsToDelete).ToList();

            Parallel.ForEach(urls,
                new ParallelOptions { MaxDegreeOfParallelism = 2 },
                url =>
                {
                    if (_pages[url].GetVisited()) return;
                    if (!url.StartsWith(_baseUrl)) return;
                    Crawl(depth + 1, url);
                });
        }

        public IReadOnlyDictionary<string, Node> GetPages()
        {
            return _pages ?? new ConcurrentDictionary<string, Node>();
        }

        private static IEnumerable<string> AddParentNode(string url, IEnumerable<string> urls)
        {
            ICollection<string> existingUrls = new List<string>();
            foreach (var u in urls)
            {
                if (_pages.ContainsKey(u))
                {
                    _pages[u].AddParentNode(url);
                    existingUrls.Add(u);
                }

                else
                {
                    var node = new Node(u)
                        .AddParentNode(url);
                    _pages.TryAdd(u, node);
                }
            }

            return existingUrls;
        }

        private IEnumerable<string> GetAttributes(string htmlTag, string attribute)
        {
            var list = new List<string>();
            _searchElements.GetBy(By.XPath($".//{htmlTag}[@{attribute}]"))
                .ForEach(e => list.Add(e.GetAttribute(attribute)));
            return list;
        }

        private void OpenPage(Node node, int waitSeconds = 1)
        {
            _webDriver.Navigate().GoToUrl(node.GetUrl());
            Wait(waitSeconds);
        }

        private static Node GetNode(string url)
        {
            Node node;
            if (_pages.ContainsKey(url))
            {
                node = _pages[url];
            }
            else
            {
                node = new Node(url);
                _pages[url] = node;
            }

            return node;
        }

        private void Wait(int waitSeconds)
        {
            if (waitSeconds <= 0)
            {
                return;
            }
            _logger.Log($"Waiting {waitSeconds} seconds.");
            Thread.Sleep(waitSeconds * 1000);
        }
    }
}