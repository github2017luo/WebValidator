using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace WebValidator.Search
{
    public class SeleniumSearchElements : ISearchElements
    {
        private readonly IWebDriver _driver;

        public SeleniumSearchElements(IWebDriver driver)
        {
            _driver = driver;
        }
        public List<IWebElement> GetBy(By by)
        {
            return _driver.FindElements(by).ToList();
        }
    }
}