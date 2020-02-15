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
        private static ConcurrentDictionary<string, Node> _pages;
        private static int? _depth;
        private static string _baseUrl;


        public HtmlCrawler(ILogger logger, int depth, string baseUrl)
        {
            _logger = logger;
            _depth ??= depth;
            _pages ??= new ConcurrentDictionary<string, Node>();
            _baseUrl = baseUrl;
        }

        public void Crawl(int depth, string url)
        {
            if (depth > _depth) return;
            var node = GetNode(url);
            var status = OpenPage(node);

            node.MakeVisited()
                .SetStatusCode(status);
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

        public IReadOnlyDictionary<string, Node> GetPages()
        {
            return _pages ?? new ConcurrentDictionary<string, Node>();
        }

        public void Dispose()
        {
            
        }
    }
}