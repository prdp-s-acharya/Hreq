using System.Text;

namespace HReq
{
    internal static class ResponsePrinter
    {
        public static void PrintResponse(this HttpResponseMessage message, Request req, string basepath, string resPath)
        {

            var resBody = message.Content.ReadAsStringAsync().Result;
            resBody = string.IsNullOrEmpty(resBody) ? null : resBody;

            Console.WriteLine("Status Code      : {0}", message.StatusCode);
            Console.WriteLine("Response Body    : \n{0}", resBody);

            string filebody = $"{{\"dateTime\":\"{DateTime.Now}\",\"request\":{req},  \"status\":\"{message.StatusCode}\",\"body\":{resBody}}}".FormatJsonString() ?? string.Empty;
            var filecont = Encoding.UTF8.GetBytes(filebody);

            if (!Directory.Exists(basepath))
            {
                Directory.CreateDirectory(basepath);
            }

            using var outfile = File.OpenWrite(basepath + "\\" + resPath);
            outfile.Write(filecont, (int)outfile.Length, filecont.Length);
            outfile.Close();
        }
    }
}
