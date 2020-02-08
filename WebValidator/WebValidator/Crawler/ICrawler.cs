using System;
using System.Collections.Generic;

namespace WebValidator.Crawler
{
    public interface ICrawler : IDisposable
    {
        void Crawl(int depth, string url);
        IReadOnlyDictionary<string, Node> GetPages();
    }
}