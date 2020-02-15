using System.Collections.Generic;
using System.Net;

namespace WebValidator
{
    public class Node
    {
        private readonly HashSet<string> _parentNodes;
        private readonly ICollection<string> _errors;
        private readonly string _url;
        private bool _visited;
        private HttpStatusCode _statusCode;

        public Node(string url)
        {
            _url = url;
            _parentNodes = new HashSet<string>();
            _errors = new List<string>();
        }

        public Node MakeVisited()
        {
            _visited = true;
            return this;
        }

        public bool GetVisited()
        {
            return _visited;
        }

        public Node AddParentNode(string url)
        {
            lock (_parentNodes)
            {
                _parentNodes.Add(url);
            }

            return this;
        }

        public string GetUrl()
        {
            return _url;
        }

        public ICollection<string> GetParentNodes()
        {
            lock (_parentNodes)
            {
                return _parentNodes;
            }
        }

        public Node AddError(string error)
        {
            lock (_errors)
            {
                _errors.Add(error); 
            }

            return this;
        }

        public IEnumerable<string> GetErrors()
        {
            lock (_errors)
            {
                return _errors;
            }
        }

        public Node SetStatusCode(HttpStatusCode status)
        {
            _statusCode = status;
            return this;
        }

        public HttpStatusCode GetStatusCode()
        {
            return _statusCode;
        }
    }
}