using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebValidator.Request;

namespace WebValidator.Validator
{
    public class Validator : IValidator
    {
        private readonly IHttpClient _httpClient;

        public Validator(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Validate(IEnumerable<Node> nodes)
        {
            Parallel.ForEach(nodes, node =>
            {
                if (node.GetVisited()) return;
                var status = _httpClient.SendHeadRequest(new Uri(node.GetUrl()));
                node.SetStatusCode(status)
                    .MakeVisited();
            });
        }
    }
}