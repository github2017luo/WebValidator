using System;
using System.Net;
using RestSharp;

namespace WebValidator.Request
{
    public class HttpClient : IHttpClient
    {
        public HttpStatusCode SendHeadRequest(Uri uri) 
        {
            var client = new RestClient(uri);
            var request = new RestRequest(Method.HEAD);

            var response = client.Execute(request);
            return response.StatusCode;
        }

        public HttpStatusCode SendGetRequest(Uri uri)
        {
            var client = new RestClient(uri);
            var request = new RestRequest(Method.GET);

            var response = client.Execute(request);
            return response.StatusCode;
        }
    }
}