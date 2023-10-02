using System.Net.Http.Json;
using System.Text.Json;

namespace HReq
{
    internal static class HttpHelper
    {
        public static HttpResponseMessage Request(Request req)
        {
            HttpClient client = new();
            HttpRequestMessage request = new(MapRequestMethodToHttpMethod(req.RequestType), new Uri(req.Url));

            foreach (var header in req.Headers)
            {

                if (header.Key == Tokens.ContentType) continue;
                request.Headers.Add(header.Key, header.Value);
            }

            if (req.RequestType == RequestMethod.POST || req.RequestType == RequestMethod.PUT)
            {
                var jsonContent = JsonContent.Create(JsonSerializer.Deserialize<Object>(req.Body), typeof(Object));
                request.Content = jsonContent;
            }

            return client.Send(request);
        }

        public static HttpMethod MapRequestMethodToHttpMethod(RequestMethod requestMethod)
        {
            return requestMethod switch
            {
                RequestMethod.GET => HttpMethod.Get,
                RequestMethod.POST => HttpMethod.Post,
                RequestMethod.PUT => HttpMethod.Put,
                RequestMethod.DELETE => HttpMethod.Delete,
            };
        }
    }
}
