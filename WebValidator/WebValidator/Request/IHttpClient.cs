using System;
using System.Net;

namespace WebValidator.Request
{
    public interface IHttpClient
    {
        HttpStatusCode SendHeadRequest(Node node);
        HttpStatusCode SendGetRequest(Uri uri);
    }
}