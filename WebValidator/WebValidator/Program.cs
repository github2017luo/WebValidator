using System;
using WebValidator.Search;
using WebValidator.SeleniumDriver;

namespace WebValidator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using var driver = new SeleniumDriverInitilizer().InitializeDriver(args[0]);
            var validator = new Validator.Validator(driver, new Uri(args[1]), new SearchElements(driver), new Request.Request());
            var errors = validator.ValidateUrls();
            if (errors.Count == 0)
            {
                Console.WriteLine("No http errors!");
                return;
            }

            foreach (var error in errors)
            {
                Console.WriteLine($"{error.Uri}, code: {error.StatusCode}");
            }
        }
    }
}
