using System.Collections.Generic;
using System.Linq;

namespace WebValidator.Util
{
    public static class Sanitizer
    {
        public static IEnumerable<string> SanitizeUrls(IEnumerable<string> urls, string baseUrl, IReadOnlyDictionary<string, bool> visitedPages)
        {
            var sanitized = urls.Distinct().Select(u =>
            {
                if (u.StartsWith("/"))
                    return baseUrl + u;
                return u;
            }).ToList();
            sanitized.RemoveAll(u => !u.StartsWith("http"));
            var visited = visitedPages.Where(p => !p.Value).Select(p => p.Key);
            sanitized = sanitized.Except(visited).ToList();
            return sanitized;
        }
    }
}