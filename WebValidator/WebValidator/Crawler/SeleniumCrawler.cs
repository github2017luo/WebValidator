using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using WebValidator.Logger;
using WebValidator.Search;
using WebValidator.SeleniumDriver;

namespace WebValidator.Crawler
{
    public class SeleniumCrawler// : ICrawler
    {
        private readonly ISearchElements _searchElements;
        private readonly ILogger _logger;
        private readonly IWebDriver _webDriver;

        public SeleniumCrawler(string browser, ILogger logger, IDriverInitializer driverInitializer)
        {
            _webDriver = driverInitializer.InitializeDriver(browser);
            _searchElements = new SeleniumSearchElements(_webDriver);
            _logger = logger;
        }

        public void OpenPage(Uri url, int waitSeconds)
        {
            _webDriver.Navigate().GoToUrl(url);
            Wait(waitSeconds);
        }

        public IEnumerable<string> GetAttributes(string htmlTag, string attribute)
        {
            var list = new List<string>();
            _searchElements.GetBy(By.XPath($".//{htmlTag}[@{attribute}]"))
                .ForEach(e => list.Add(e.GetAttribute(attribute)));
            _logger.Log($"Found {list.Count} elements");
            return list;
        }

        public void Dispose()
        {
            _logger.Log("Dispose of browser.");
            _webDriver?.Dispose();
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