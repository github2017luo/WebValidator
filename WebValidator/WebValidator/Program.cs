﻿using System;
using WebValidator.Logger;
using WebValidator.Search;
using WebValidator.SeleniumDriver;

namespace WebValidator
{
    internal class Program
    {
        private static readonly ILogger Logger = new ConsoleLogger();
        private static void Main(string[] args)
        {
            using var driver = new SeleniumDriverInitilizer().InitializeDriver(args[0]);
            var validator = new Validator.Validator(driver,
                new Uri(args[1]),
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
        }
    }
}
