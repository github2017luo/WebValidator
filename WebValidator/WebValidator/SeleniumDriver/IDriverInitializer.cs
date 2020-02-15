using System;
using OpenQA.Selenium;

namespace WebValidator.SeleniumDriver
{
    public interface IDriverInitializer
    {
        IWebDriver InitializeDriver(string browser);
    }
}