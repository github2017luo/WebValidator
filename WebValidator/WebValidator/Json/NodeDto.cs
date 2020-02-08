using System.Collections.Generic;
using System.Net;

namespace WebValidator.Json
{
    public class NodeDto
    {
        public List<string> ParentNodes;
        public string Url { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}