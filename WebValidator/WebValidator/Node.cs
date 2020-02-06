using System.Collections.Generic;

namespace WebValidator
{
    public class Node
    {
        private readonly HashSet<string> _parentNodes;
        private readonly string _path;
        private bool _visited;

        public Node(string path)
        {
            _path = path;
            _parentNodes = new HashSet<string>();
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

        public string GetPath()
        {
            return _path;
        }

        public ICollection<string> GetParentNodes()
        {
            lock (_parentNodes)
            {
                return _parentNodes;
            }
        }
    }
}