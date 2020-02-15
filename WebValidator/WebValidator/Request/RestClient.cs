using RestSharp;

namespace WebValidator.Request
{
    public class RestClient : IHttpClient
    {
        public IRestResponse SendHeadRequest(Node node)
        {
            var client = new RestSharp.RestClient(node.GetUrl());
            var request = new RestRequest(Method.HEAD);

            var response = client.Execute(request);
            return response;
        }
    }
}