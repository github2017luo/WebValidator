using RestSharp;
using System;
using System.Net;

namespace WebValidator.Request
{
    public class RestClient : IHttpClient
    {
        public HttpStatusCode SendHeadRequest(Node node)
        {
            var client = new RestSharp.RestClient(node.GetUrl());
            var request = new RestRequest(Method.HEAD);

            var response = client.Execute(request);
            return response.StatusCode;
        }

        public HttpStatusCode SendGetRequest(Uri uri)
        {
            var client = new RestSharp.RestClient(uri);
            var request = new RestRequest(Method.GET);

            var response = client.Execute(request);
            return response.StatusCode;
        }
    }
}