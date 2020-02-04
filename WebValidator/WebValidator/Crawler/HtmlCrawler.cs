using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WebValidator.Logger;
using WebValidator.Util;

namespace WebValidator.Crawler
{
    public class HtmlCrawler : ICrawler
    {
        private readonly ILogger _logger;
        private HtmlDocument _htmlDoc;
        private static ConcurrentDictionary<Uri, string> _errors;
        private static ConcurrentDictionary<string, bool> _visitedPages;
        private static int? _depth;
        private static string _baseUrl;


        public HtmlCrawler(ILogger logger, int depth, string baseUrl)
        {
            _logger = logger;
            _depth ??= depth;
            _visitedPages ??= new ConcurrentDictionary<string, bool>();
            _baseUrl = baseUrl;
            _errors ??= new ConcurrentDictionary<Uri, string>();
        }


        public void OpenPage(Uri url, int waitSeconds = 0)
        {
            try
            {
                _htmlDoc = new HtmlWeb().Load(url);
            }
            catch (ArgumentException e)
            {
                _errors.TryAdd(url, e.Message);
                _htmlDoc = new HtmlDocument();
            }
            catch (System.Net.WebException e)
            {
                _errors.TryAdd(url, e.Message);
                _htmlDoc = new HtmlDocument();
            }

            Wait(waitSeconds);
        }

        public IEnumerable<string> GetAttributes(string htmlTag, string attribute)
        {
            var list = new List<string>();
            _htmlDoc.DocumentNode.SelectNodes($".//{htmlTag}[@{attribute}]")?.ToList().ForEach(e => list.Add(e.GetAttributeValue(attribute,"no attribute")));
            //_logger.Log($"Found {list.Count} elements");
            return list;
        }

        public void Crawl(int depth, string url)
        {
            if (depth > _depth)
                return;
            //new ConsoleLogger().Log("Working on: " + url + " Depth: " + depth);
            OpenPage(new Uri(url));
            _visitedPages[url] = true;
            var urls = GetAttributes("a", "href").ToList();

            urls = Sanitizer.SanitizeUrls(urls, _baseUrl, _visitedPages).ToList();

            urls.ForEach(u => _visitedPages.TryAdd(u, false));
            Parallel.ForEach(urls,
                //new ParallelOptions {MaxDegreeOfParallelism = 4},
                url =>
                {
                    if (_visitedPages[url]) return;
                    if (!url.StartsWith(_baseUrl)) return;
                    Crawl(depth + 1, url);
                });
        }

        public IReadOnlyDictionary<string, bool> GetVisitedPages()
        {
            return _visitedPages ?? new ConcurrentDictionary<string, bool>();
        }

        public void Dispose()
        {
            
        }

        public IDictionary<Uri, string> GetErrors()
        {
            return _errors;
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