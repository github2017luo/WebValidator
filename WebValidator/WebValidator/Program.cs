using System;
using System.Collections.Generic;
using WebValidator.Logger;
using WebValidator.Page;
using WebValidator.SeleniumDriver;

namespace WebValidator
{
    internal class Program
    {
        private static readonly ILogger Logger = new ConsoleLogger();
        private static string _browser;
        private static IPage _page;
        private static void Main(string[] args)
        {
            _browser = args[0];
            _page = new Page.Page(_browser, new ConsoleLogger(), new SeleniumDriverInitilizer());
            var urls = Check(args[1]);
            //var deep = Int32.Parse(args[2]);
            //if (deep <= 0) return;
            //for (var i = 0; i <= deep; ++i)
            //{
            //    foreach (var url in urls)
            //    {
            //        Check(url);
            //    }
            //}
            
            //Parallel.ForEach(urls, url => { ValidatePage(_browser, url).Dispose(); });
        }

        private static ICollection<string> Check(string url)
        {
            _page.OpenPage(new Uri(url), 5);
            var urls = _page.GetAttributes("a", "href");
            var errors = new Validator.Validator(new Request.Request(), new ConsoleLogger()).ValidateUrls(urls);
            Logger.LogErrors(errors);
            return urls;
        }

        //private static IWebDriver ValidatePage(string browser, string url)
        //{
        //    Logger.Log("Validation of " + url);
        //    var driver = new SeleniumDriverInitilizer().InitializeDriver(browser);
        //    var validator = new Validator.Validator(new SearchElements(driver),
        //        new Request.Request(),
        //        new ConsoleLogger());
        //    Logger.Log(DateTime.Now + " Checking URLs.");
        //    var errors = validator.ValidateUrls();
        //    Logger.LogErrors(errors);
        //    Logger.Log(DateTime.Now + " Checking images.");
        //    errors = validator.ValidateImages();
        //    Logger.LogErrors(errors);
        //    return driver;
        //}
    }
}
