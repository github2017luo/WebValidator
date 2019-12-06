using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium;
using WebValidator.Logger;
using WebValidator.Search;
using WebValidator.SeleniumDriver;

namespace WebValidator
{
    internal class Program
    {
        private static readonly ILogger Logger = new ConsoleLogger();
        private static string _browser;
        private static void Main(string[] args)
        {
            _browser = args[0];
            var driver = ValidatePage(_browser, args[1]);
            var elements = new SearchElements(driver).GetBy(By.XPath(".//*[@href]"));
            var urls = new List<string>();
            foreach (var webElement in elements)
            {
                urls.Add(webElement.GetAttribute("href"));
            }
            driver.Dispose();
            Parallel.ForEach(urls, url => { ValidatePage(_browser, url).Dispose(); });
        }

        private static IWebDriver ValidatePage(string browser, string url)
        {
            Logger.Log("Validation of " + url);
            var driver = new SeleniumDriverInitilizer().InitializeDriver(browser);
            var validator = new Validator.Validator(driver,
                new Uri(url),
                new SearchElements(driver),
                new Request.Request(),
                5,
                new ConsoleLogger());
            Logger.Log(DateTime.Now + " Checking URLs.");
            var errors = validator.ValidateUrls();
            Logger.LogErrors(errors);
            Logger.Log(DateTime.Now + " Checking images.");
            errors = validator.ValidateImages();
            Logger.LogErrors(errors);
            return driver;
        }
    }
}
