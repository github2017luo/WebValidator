using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace WebValidator.Search
{
    public class SearchElements : ISearchElements
    {
        private readonly IWebDriver _driver;

        public SearchElements(IWebDriver driver)
        {
            _driver = driver;
        }
        public List<IWebElement> GetBy(By by)
        {
            return _driver.FindElements(by).ToList();
        }
    }
}