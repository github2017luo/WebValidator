using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace WebValidator.Page
{
    public interface IPage
    {
        IWebDriver OpenPage(Uri url, int waitSeconds);
        ICollection<string> GetAttributes(string htmlTag, string attribute);
    }
}