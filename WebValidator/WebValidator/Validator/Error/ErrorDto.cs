using System;
using OpenQA.Selenium;

namespace WebValidator.Validator.Error
{
    public class ErrorDto
    {
        public int StatusCode { get; set; }
        public Uri Uri { get; set; }
        public IWebElement Element { get; set; }
    }
}