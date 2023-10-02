namespace HReq
{
    internal static class Parser
    {
        public static Request ParseFile(string file, Dictionary<string, string> ver)
        {

            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException(nameof(file));
            var fileData = File.ReadAllText(file);
            fileData = fileData.Trim().FilterComments();
            var req = Request.Init(fileData);

            while (!string.IsNullOrEmpty(req.FileData))
            {
                req = req.ParseVar().Parsetype().ParseHeader().ParseBody();
            }

            req = req.ReplaceVar(ver);
            return req;
        }

        public static Dictionary<string, string> ParseVarFile(string file)
        {

            if (string.IsNullOrEmpty(file)) return new Dictionary<string, string>();
            var fileData = File.ReadAllLines(file);
            var ver = new Dictionary<string, string>();

            foreach (var line in fileData)
            {
                var l = line.Split('=');
                ver.Add(l[0].Trim(), l[1].Trim());
            }

            return ver;
        }
    }
}
