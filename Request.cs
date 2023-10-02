using System.Text;
using System.Text.Json;

namespace HReq
{
    internal class Request
    {
        public string Url { get; set; }

        public RequestMethod RequestType { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public Dictionary<string, string> Var { get; set; }

        public string? Body { get; set; }



        public string FileData { get; set; }

        public Request(string filData)
        {
            FileData = filData;
            Headers = new();
            Var = new();
        }

        public static Request Init(string fileData)
        {
            return new Request(fileData);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb = sb.Append('{')
                .Append($"\"url\" : \"{this.Url}\",")
                .Append($"\"httpMethod\":\"{this.RequestType.ToString()}\",")
                .Append($"\"body\":{this.Body?.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ').Replace("\u0022", "\"") ?? string.Empty},")
                .Append($"\"header\":{JsonSerializer.Serialize(this.Headers)}}}");
            return sb.ToString();
        }
    }
}
