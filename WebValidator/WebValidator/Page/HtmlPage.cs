using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WebValidator.Logger;
using WebValidator.Search;

namespace WebValidator.Page
{
    public class HtmlPage : IPage, IDisposable
    {
        private readonly ISearchElements _searchElements;
        private readonly ILogger _logger;
        private HtmlDocument _htmlDoc;
        private static ConcurrentDictionary<Uri, string> _errors;


        public HtmlPage(ILogger logger)
        {
            _logger = logger;
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

        public ICollection<string> GetAttributes(string htmlTag, string attribute)
        {
            var list = new List<string>();
            _htmlDoc.DocumentNode.SelectNodes($".//{htmlTag}[@{attribute}]")?.ToList().ForEach(e => list.Add(e.GetAttributeValue(attribute,"")));
            _logger.Log($"Found {list.Count} elements");
            return list;
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