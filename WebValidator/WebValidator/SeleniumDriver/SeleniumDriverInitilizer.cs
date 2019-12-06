using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebValidator.Common;

namespace WebValidator.SeleniumDriver
{
    public class SeleniumDriverInitilizer : IDriverInitializer
    {
        public IWebDriver InitializeDriver(string browser)
        {
            switch (browser)
            {
                case BrowserType.Chrome:
                    return GetChrome();
                default:
                    throw new ArgumentException("Wrong browser type");
            }
        }

        private static IWebDriver GetChrome()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            return new ChromeDriver(options);
        }
    }
}