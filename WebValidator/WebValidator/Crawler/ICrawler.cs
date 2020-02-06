using System;
using System.Collections.Generic;

namespace WebValidator.Crawler
{
    public interface ICrawler : IDisposable
    {
        void OpenPage(Uri url);
        IEnumerable<string> GetAttributes(string htmlTag, string attribute);
        void Crawl(int depth, string url);
        IReadOnlyDictionary<string, Node> GetVisitedPages();
    }
}