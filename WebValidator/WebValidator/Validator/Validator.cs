using RestSharp;
using System.Collections.Generic;
using System.Threading;
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
                var response = _httpClient.SendHeadRequest(node);
                if (response.StatusCode == default)
                {
                    if (response.ErrorMessage.Equals(
                        "An error occurred while sending the request. The response ended prematurely."))
                    {
                        response = SendRequest(node);
                    }
                    if (response.StatusCode == default)
                    {
                        node.AddError(response.ErrorMessage);
                    }
                }

                node.SetStatusCode(response.StatusCode)
                    .MakeVisited();
            });
        }

        private IRestResponse SendRequest(Node node)
        {
            Thread.Sleep(500);
            var response = _httpClient.SendHeadRequest(node);
            return response;
        }
    }
}