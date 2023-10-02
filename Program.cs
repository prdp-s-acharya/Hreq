namespace HReq
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var basepath = "root path of file";
            var verPath = basepath + "v1.var";
            var reqFileName = "filename.req";
            var reqPath = basepath + reqFileName;
            var resPath = reqFileName.Replace(".req", $"{DateTime.Now:HHmmss}.res");

            Dictionary<string, string> ver = Parser.ParseVarFile(verPath);
            var req = Parser.ParseFile(reqPath, ver);

            var response = HttpHelper.Request(req);
            response.PrintResponse(req, basepath + $"{reqFileName.Remove(reqFileName.Length - 4)}", resPath);

        }
    }
}