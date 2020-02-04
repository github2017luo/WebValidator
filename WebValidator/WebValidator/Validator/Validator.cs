using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebValidator.Logger;
using WebValidator.Request;
using WebValidator.Validator.Error;

namespace WebValidator.Validator
{
    public class Validator : IValidator
    {
        private readonly IRequest _request;
        private readonly ILogger _logger;
        private static ConcurrentDictionary<string, bool> _visitedPages;

        public Validator(IRequest request, ILogger logger)
        {
            _request = request;
            _logger = logger;
            _visitedPages ??= new ConcurrentDictionary<string, bool>();
        }

        //public List<ErrorDto> ValidateUrls(ICollection<string> urls)
        //{
        //    var errorList = new List<ErrorDto>();
        //    Parallel.ForEach(urls, url =>
        //        //foreach (var url in urls)
        //    {
        //        //_logger.Log("Working on: " + url);

        //        if (AlreadyVisited(url))
        //        {
        //            _logger.Log("SeleniumPage already visited.");
        //            return; //continue;
        //        }

        //        var status = _request.SendHeadRequest(new Uri(url));
        //        if (IsErrorStatus(status))
        //        {
        //            AddError(errorList, status, url);
        //        }

        //        //TODO: Handle the false case?
        //        if (_visitedPages.TryAdd(url, true))
        //            _logger.Log("Success! Added " + url);
        //    });//}
        //    return errorList;
        //}

        public List<ErrorDto> Validate(IEnumerable<string> urls)
        {
            var errorList = new List<ErrorDto>();
            Parallel.ForEach(urls, url =>
                //foreach (var url in urls)
            {
                //_logger.Log("Working on: " + url);

                if (AlreadyVisited(url))
                {
                    _logger.Log($"Url {url} already visited.");
                    return; //continue;
                }

                var status = _request.SendHeadRequest(new Uri(url));
                if (IsErrorStatus(status))
                {
                    AddError(errorList, status, url);
                }

                //TODO: Handle the false case?
                if (_visitedPages.TryAdd(url, true))
                    _logger.Log("Success! Added " + url);
            });//}
            return errorList;
        }

        //public List<ErrorDto> ValidateImages()
        //{
        //    var elements = _search.GetBy(By.XPath(".//img[@src]"));
        //    _logger.Log($"Found {elements.Count} images.");

        //    var errorList = new List<ErrorDto>();
        //    Parallel.ForEach(elements, webElement =>
        //    {
        //        var uri = webElement.GetAttribute("src");
        //        var status = _request.SendHeadRequest(new Uri(uri));
        //        if (status >= 300 && status <= 599)
        //        {
        //            errorList.Add(new ErrorDto
        //            {
        //                //Element = webElement,
        //                StatusCode = status,
        //                Uri = uri
        //            });
        //        }
        //    });
        //    return errorList;
        //}

        private static bool IsErrorStatus(int status)
        {
            return status >= 300 && status <= 599;
        }

        private static bool AlreadyVisited(string uri)
        {
            return _visitedPages.ContainsKey(uri) && _visitedPages[uri];
        }

        private static void AddError(ICollection<ErrorDto> errorList, int status, string uri)
        {
            errorList.Add(new ErrorDto
            {
                StatusCode = status,
                Uri = uri
            });
        }

        public List<ErrorDto> ValidateUrls(ICollection<string> urls)
        {
            throw new NotImplementedException();
        }
    }
}