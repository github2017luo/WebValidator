using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private static ConcurrentDictionary<string, Node> _visitedPages;
        private static int? _depth;
        private static string _baseUrl;


        public HtmlCrawler(ILogger logger, int depth, string baseUrl)
        {
            _logger = logger;
            _depth ??= depth;
            _visitedPages ??= new ConcurrentDictionary<string, Node>();
            _baseUrl = baseUrl;
        }

        public void Crawl(int depth, string url)
        {
            var node = _visitedPages.ContainsKey(url) ? _visitedPages[url] : new Node(url);
            if (depth > _depth) return;
            var status = OpenPage(node);

            MakeUrlVisited(url);
            node.SetStatusCode(status);
            
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

        private static IEnumerable<string> AddParentNode(string url, IEnumerable<string> urls)
        {
            ICollection<string> existingUrls = new List<string>();
            foreach (var u in urls)
            {
                if (_visitedPages.ContainsKey(u))
                {
                    _visitedPages[u].AddParentNode(url);
                    existingUrls.Add(u);
                }

                else
                {
                    var node = new Node(u)
                        .AddParentNode(url);
                    _visitedPages.TryAdd(u, node);
                }
            }

            return existingUrls;
        }

        private HttpStatusCode OpenPage(Node node)
        {
            try
            {
                return LoadPage(node);
            }
            catch (Exception)
            {
                // Try to load the page once more time, before reporting an error.
                try
                {
                    return LoadPage(node);
                }
                catch (Exception e)
                {
                    node.AddError(e.Message)
                        .AddError(e.ToString());
                    return default;
                }
            }
        }

        private HttpStatusCode LoadPage(Node node)
        {
            var web = new HtmlWeb();
            _htmlDoc = web.Load(node.GetUrl());
            return web.StatusCode;
        }

        private IEnumerable<string> GetAttributes(string htmlTag, string attribute)
        {
            var list = new List<string>();
            _htmlDoc.DocumentNode.SelectNodes($".//{htmlTag}[@{attribute}]")
                ?.ToList()
                .ForEach(e => list.Add(e.GetAttributeValue(attribute,
                    "no attribute")));
            return list;
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

        public IReadOnlyDictionary<string, Node> GetPages()
        {
            return _visitedPages ?? new ConcurrentDictionary<string, Node>();
        }

        public void Dispose()
        {
            
        }
    }
}