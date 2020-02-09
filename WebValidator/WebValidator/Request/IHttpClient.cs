using System;
using System.Net;

namespace WebValidator.Request
{
    public interface IHttpClient
    {
        HttpStatusCode SendHeadRequest(Uri uri);
        HttpStatusCode SendGetRequest(Uri uri);
    }
}