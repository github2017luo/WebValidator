using System.Collections.Generic;
using System.Net;

namespace WebValidator.Json
{
    public class NodeDto
    {
        public string Url { get; set; }
        public List<string> ParentNodes;
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; }
        public bool Visited { get; set; }
    }
}