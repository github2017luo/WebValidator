using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using WebValidator.Logger;
using WebValidator.Search;
using WebValidator.SeleniumDriver;

namespace WebValidator.Page
{
    public class Page : IPage
    {
        private readonly ISearchElements _searchElements;
        private readonly ILogger _logger;
        private readonly IWebDriver _webDriver;

        public Page(string browser, ILogger logger, IDriverInitializer driverInitializer)
        {
            _webDriver = driverInitializer.InitializeDriver(browser);
            _searchElements = new SearchElements(_webDriver);
            _logger = logger;
        }

        public IWebDriver OpenPage(Uri url, int waitSeconds)
        {
            _webDriver.Navigate().GoToUrl(url);
            Wait(waitSeconds);
            return _webDriver;
        }

        public ICollection<string> GetAttributes(string htmlTag, string attribute)
        {
            var list = new List<string>();
            _searchElements.GetBy(By.XPath($".//{htmlTag}[@{attribute}]"))
                .ForEach(e => list.Add(e.GetAttribute(attribute)));
            _logger.Log($"Found {list.Count} elements");
            return list;
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