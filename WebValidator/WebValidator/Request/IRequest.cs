using System;

namespace WebValidator.Request
{
    public interface IRequest
    {
        int SendHeadRequest(Uri uri);
        int SendGetRequest(Uri uri);
    }
}