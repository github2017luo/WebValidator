using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private static ConcurrentDictionary<string, Node> _visitedPages;
        private static int? _depth;
        private static string _baseUrl;


        public HtmlCrawler(ILogger logger, int depth, string baseUrl)
        {
            _logger = logger;
            _depth ??= depth;
            _visitedPages ??= new ConcurrentDictionary<string, Node>();
            _baseUrl = baseUrl;
            _errors ??= new ConcurrentDictionary<Uri, string>();
        }


        public void OpenPage(Uri url)
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
        }

        public IEnumerable<string> GetAttributes(string htmlTag, string attribute)
        {
            var list = new List<string>();
            _htmlDoc.DocumentNode.SelectNodes($".//{htmlTag}[@{attribute}]")
                ?.ToList()
                .ForEach(e => list.Add(e.GetAttributeValue(attribute,
                    "no attribute")));
            return list;
        }

        public void Crawl(int depth, string url)
        {
            if (depth > _depth) return;
            OpenPage(new Uri(url));

            MakeUrlVisited(url);
            
            var urls = GetAttributes("a", "href").ToList();
            urls = Sanitizer.SanitizeUrls(urls, _baseUrl, _visitedPages).ToList();

            var urlsToDelete = AddParentNode(url, urls);
            urls = urls.Except(urlsToDelete).ToList();

            Parallel.ForEach(urls,
                url =>
                {
                    if (_visitedPages[url].GetVisited()) return;
                    if (!url.StartsWith(_baseUrl)) return;
                    Crawl(depth + 1, url);
                });
        }

        private static ICollection<string> AddParentNode(string url, List<string> urls)
        {
            ICollection<string> urlsToDelete = new List<string>();
            foreach (var u in urls)
            {
                if (_visitedPages.ContainsKey(u))
                {
                    _visitedPages[u].AddParentNode(url);
                    urlsToDelete.Add(u);
                }

                else
                {
                    var node = new Node(u)
                        .AddParentNode(url);
                    _visitedPages.TryAdd(u, node);
                }
            }

            return urlsToDelete;
        }

        private static void MakeUrlVisited(string url)
        {
            if (_visitedPages.ContainsKey(url))
            {
                _visitedPages[url].MakeVisited();
            }
            else
            {
                _visitedPages[url] = new Node(url).MakeVisited();
            }
        }

        public IReadOnlyDictionary<string, Node> GetVisitedPages()
        {
            return _visitedPages ?? new ConcurrentDictionary<string, Node>();
        }

        public void Dispose()
        {
            
        }

        public IDictionary<Uri, string> GetErrors()
        {
            return _errors;
        }
    }
}