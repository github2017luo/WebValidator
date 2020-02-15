using System.Collections.Generic;
using System.Linq;

namespace WebValidator.Util
{
    public static class Sanitizer
    {
        public static IEnumerable<string> SanitizeUrls(IEnumerable<string> urls, string baseUrl, IReadOnlyDictionary<string, Node> visitedPages)
        {
            var sanitized = urls.Distinct().Select(u =>
            {
                if (u.StartsWith("/"))
                    return baseUrl + u;
                return u;
            }).ToList();
            sanitized.RemoveAll(u => !u.StartsWith("http"));
            return sanitized;
        }
    }
}