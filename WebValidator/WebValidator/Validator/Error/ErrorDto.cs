using OpenQA.Selenium;

namespace WebValidator.Validator.Error
{
    public class ErrorDto
    {
        public int StatusCode { get; set; }
        public string Uri { get; set; }
        public IWebElement Element { get; set; }
    }
}