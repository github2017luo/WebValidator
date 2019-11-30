using System;
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

        public Validator(IWebDriver driver, Uri uri, ISearchElements search, IRequest request, int waitSeconds, ILogger logger)
        {
            _search = search;
            _request = request;
            _logger = logger;
            driver.Navigate().GoToUrl(uri);
            _logger.Log($"Waiting {waitSeconds} seconds.");
            Thread.Sleep(waitSeconds * 1000);
        }
        public List<ErrorDto> ValidateUrls()
        {
            var elements = _search.GetBy(By.XPath(".//*[@href]"));
            _logger.Log($"Found {elements.Count} urls.");
            var errorList = new List<ErrorDto>();
            Parallel.ForEach(elements, webElement =>
            {
                var uri = new Uri(webElement.GetAttribute("href"));
                var status = _request.SendHeadRequest(uri);
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

        // TODO: Check how it behaves with base 64 encoded content. 
        public List<ErrorDto> ValidateImages()
        {
            var elements = _search.GetBy(By.XPath(".//img[@src]"));
            _logger.Log($"Found {elements.Count} images.");

            var errorList = new List<ErrorDto>();
            Parallel.ForEach(elements, webElement =>
            {
                var uri = new Uri(webElement.GetAttribute("src"));
                var status = _request.SendHeadRequest(uri);
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