using System;
using RestSharp;

namespace WebValidator.Request
{
    public class Request : IRequest
    {
        public int SendHeadRequest(Uri uri) 
        {
            var client = new RestClient(uri);
            var request = new RestRequest(Method.HEAD);

            var response = client.Execute(request);
            return (int)response.StatusCode;
        }

        public int SendGetRequest(Uri uri)
        {
            var client = new RestClient(uri);
            var request = new RestRequest(Method.GET);

            var response = client.Execute(request);
            return (int)response.StatusCode;
        }
    }
}