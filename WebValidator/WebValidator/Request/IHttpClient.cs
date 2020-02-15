using RestSharp;

namespace WebValidator.Request
{
    public interface IHttpClient
    {
        IRestResponse SendHeadRequest(Node node);
    }
}