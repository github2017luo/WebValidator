using System;
using System.Collections.Generic;
using System.Linq;
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

            //foreach (var node in nodes)
            //{
            //    //Console.Write(node.GetUrl() + node.GetVisited());
            //    if (node.GetVisited())
            //    {
            //        Console.WriteLine("Already visited!");
            //        return;
            //    }
            //    var status = _httpClient.SendHeadRequest(node);
            //    //Console.Write(status);
            //    node.SetStatusCode(status)
            //        .MakeVisited();

            //    //Console.WriteLine(node.GetVisited());
            //}

            Parallel.ForEach(nodes, node =>
            {
                if (node.GetVisited()) return;
                var status = _httpClient.SendHeadRequest(node);
                node.SetStatusCode(status)
                    .MakeVisited();
            });
        }
    }
}