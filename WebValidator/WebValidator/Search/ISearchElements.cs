using System.Collections.Generic;
using OpenQA.Selenium;

namespace WebValidator.Search
{
    public interface ISearchElements
    {
        List<IWebElement> GetBy(By by);
    }
}