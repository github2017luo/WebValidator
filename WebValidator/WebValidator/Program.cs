using System;
using WebValidator.Search;
using WebValidator.SeleniumDriver;

namespace WebValidator
{
    class Program
    {

        static void Main(string[] args)
        {
            using var driver = new SeleniumDriverInitilizer().InitializeDriver(args[0]);
            var validator = new Validator.Validator(driver, new Uri(args[1]), new SearchElements(driver), new Request.Request());
            var results = validator.ValidateUrls();
            if (results.Count == 0)
            {
                Console.WriteLine("No http errors!");
                return;
            }

            foreach (var (code, uri) in results)
            {
                Console.WriteLine($"{uri}, code: {code}");
            }
            //driver.Navigate().GoToUrl(args[1]);
            //var elements = new SearchElements(driver).GetBy(By.XPath(".//*[@href]"));
            //foreach (var element in elements)
            //{
            //    var uri = new Uri(element.GetAttribute("href"));
            //    Console.WriteLine(uri);
            //    Console.WriteLine(new Request.Request().SendHeadRequest(uri));
            //}
        }
    }
}
