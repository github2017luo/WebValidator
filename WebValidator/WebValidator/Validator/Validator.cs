using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using WebValidator.Logger;
using WebValidator.Request;
using WebValidator.Search;
using WebValidator.Validator.Error;

namespace WebValidator.Validator
{
    public class Validator : IValidator
    {
        private readonly ISearchElements _search;
        private readonly IRequest _request;
        private readonly ILogger _logger;
        private static ConcurrentDictionary<string, bool> _visitedPages;

        public Validator(IWebDriver driver, Uri uri, ISearchElements search, IRequest request, int waitSeconds, ILogger logger)
        {
            _search = search;
            _request = request;
            _logger = logger;
            _visitedPages ??= new ConcurrentDictionary<string, bool>();
            driver.Navigate().GoToUrl(uri);
            Wait(waitSeconds);
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

        public List<ErrorDto> ValidateUrls()
        {
            var elements = _search.GetBy(By.XPath(".//a[@href]"));
            _logger.Log($"Found {elements.Count} urls.");
            var errorList = new List<ErrorDto>();
            //Parallel.ForEach(elements, webElement =>
            foreach (var webElement in elements)
            {
                string uri;
                try
                {
                    uri = webElement.GetAttribute("href");
                }
                catch (UriFormatException)
                {
                    AddError(errorList, webElement, 404, null);
                    continue;//return;
                }

                _logger.Log("Working on: " + uri);

                if (AlreadyVisited(uri)) continue;//return;

                var status = _request.SendHeadRequest(new Uri(uri));
                if (IsErrorStatus(status))
                {
                    AddError(errorList, webElement, status, uri);
                }

                //TODO: Handle the false case?
                if (_visitedPages.TryAdd(uri, true))
                    _logger.Log("Added " + uri);
            }//);
            return errorList;
        }

        private static bool IsErrorStatus(int status)
        {
            return status >= 300 && status <= 599;
        }

        private static bool AlreadyVisited(string uri)
        {
            return _visitedPages.ContainsKey(uri) && _visitedPages[uri];
        }

        private static void AddError(List<ErrorDto> errorList, IWebElement webElement, int status, string uri)
        {
            errorList.Add(new ErrorDto
            {
                Element = webElement,
                StatusCode = status,
                Uri = uri
            });
        }


        // TODO: Check how it behaves with base 64 encoded content. 
        public List<ErrorDto> ValidateImages()
        {
            var elements = _search.GetBy(By.XPath(".//img[@src]"));
            _logger.Log($"Found {elements.Count} images.");

            var errorList = new List<ErrorDto>();
            Parallel.ForEach(elements, webElement =>
            {
                var uri =webElement.GetAttribute("src");
                var status = _request.SendHeadRequest(new Uri(uri));
                if (status >= 300 && status <= 599)
                {
                    errorList.Add(new ErrorDto
                    {
                        Element = webElement,
                        StatusCode = status,
                        Uri = uri
                    });
                }
            });
            return errorList;
        }


    }
}