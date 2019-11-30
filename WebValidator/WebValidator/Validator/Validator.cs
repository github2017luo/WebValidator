﻿using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using WebValidator.Request;
using WebValidator.Search;
using WebValidator.Validator.Error;

namespace WebValidator.Validator
{
    public class Validator : IValidator
    {
        private readonly ISearchElements _search;
        private readonly IRequest _request;

        public Validator(IWebDriver driver, Uri uri, ISearchElements search, IRequest request)
        {
            _search = search;
            _request = request;
            driver.Navigate().GoToUrl(uri);
        }
        public List<ErrorDto> ValidateUrls()
        {
            var elements = _search.GetBy(By.XPath(".//*[@href]"));
            var errorList = new List<ErrorDto>();
            foreach (var webElement in elements)
            {
                var uri = new Uri(webElement.GetAttribute("href"));
                var status = _request.SendHeadRequest(uri);
                if (status >= 300 && status <= 599)
                {
                    errorList.Add(new ErrorDto()
                    {
                        Element = webElement,
                        StatusCode = status,
                        Uri = uri
                    });
                }
            }
            return errorList;
        }

        public List<Tuple<int, Uri>> ValidateImages()
        {
            var elements = _search.GetBy(By.XPath(".//img[@src]"));
            var errorList = new List<Tuple<int, Uri>>();
            foreach (var webElement in elements)
            {
                var uri = new Uri(webElement.GetAttribute("href"));
                var status = _request.SendHeadRequest(uri);
                if (status >= 300 && status <= 599)
                {
                    errorList.Add(Tuple.Create(status, uri));
                }
            }
            return errorList;
        }
    }
}