using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebValidator.Logger;
using WebValidator.Page;
using WebValidator.SeleniumDriver;

namespace WebValidator
{
    public class Bfs
    {
        private static ConcurrentDictionary<string, bool> _visitedPages;
        private static int? _depth;
        private static string _baseUrl;

        public Bfs(int depth, string baseUrl)
        {
            _depth ??= depth;
            _visitedPages ??= new ConcurrentDictionary<string, bool>();
            _baseUrl = baseUrl;
        }

        public void Search(int depth, string browser, ILogger logger, IDriverInitializer driverInitializer, string url)
        {
            if (depth > _depth)
                return;
            new ConsoleLogger().Log("Working on: " + url + " Depth: " + depth);
            var urls = new List<string>();
            using (var page = new Page.SeleniumPage(browser, logger, driverInitializer))
            {
                page.OpenPage(new Uri(url), 2);
                _visitedPages[url] = true;
                urls = page.GetAttributes("a", "href").ToList();
                urls.RemoveAll(u => u.Contains(';'));
            }
            
            urls.ForEach(u => _visitedPages.TryAdd(u, false));
            Parallel.ForEach(urls,
                new ParallelOptions() {MaxDegreeOfParallelism = 4},
                url =>
                {
                    if (_visitedPages[url]) return;
                    Search(depth + 1, browser, logger, driverInitializer, url);
                });
        }

        public void Search(int depth, ILogger logger, string url)
        {
            if (depth > _depth)
                return;
            new ConsoleLogger().Log("Working on: " + url + " Depth: " + depth);
            List<string> urls;
            using (var page = new Page.HtmlPage(logger))
            {
                page.OpenPage(new Uri(url));
                _visitedPages[url] = true;
                urls = page.GetAttributes("a", "href").ToList();
            }

            urls = SanitizeUrls(urls);

            urls.ForEach(u => _visitedPages.TryAdd(u, false));
            Parallel.ForEach(urls,
                //new ParallelOptions {MaxDegreeOfParallelism = 4},
                url =>
                {
                    if (_visitedPages[url]) return;
                    if (!url.StartsWith(_baseUrl)) return;
                    Search(depth + 1, logger, url);
                });
        }

        public ConcurrentDictionary<string, bool> GetVisitedPages()
        {
            return _visitedPages ?? new ConcurrentDictionary<string, bool>();
        }

        public IDictionary<Uri, string> GetErrors()
        {
            return new HtmlPage(new ConsoleLogger()).GetErrors();
        }

        private List<string> SanitizeUrls(IEnumerable<string> urls)
        {
            var sanitized = urls.Distinct().Select(u =>
            {
                if (u.StartsWith("/"))
                    return _baseUrl + u;
                return u;
            }).ToList();
            sanitized.RemoveAll(u =>
            {
                return !u.StartsWith("http");
                //return !(Uri.TryCreate(u, UriKind.RelativeOrAbsolute, out var uri) &&
                //         (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps));
                

                //return u.Contains('#');
            });
            var visited = _visitedPages.Where(p => !p.Value).Select(p => p.Key);
            sanitized = sanitized.Except(visited).ToList();
            return sanitized;
        }
    }
}