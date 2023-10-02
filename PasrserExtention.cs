using System.Text;
using System.Text.RegularExpressions;

namespace HReq
{
    internal static class PasrserExtention
    {
        public static Request ParseVar(this Request req)
        {

            req.FileData = req.FileData.TrimStart();
            if (req.FileData.StartsWith('@'))
            {
                int charloc = req.FileData.IndexOf('\n');
                var line = req.FileData[..charloc];

                req.FileData = req.FileData[charloc..];

                var keyVal = line.Split('=');
                var key = keyVal[0].Trim().Trim('@');
                var val = keyVal[1].Trim();
                req.Var.Add(key, val);

            }
            return req;
        }

        public static Request Parsetype(this Request req)
        {
            req.FileData = req.FileData.TrimStart();

            foreach (var item in typeof(RequestMethod).GetEnumNames())
            {
                if (req.FileData.StartsWith(item))
                {
                    int charloc = req.FileData.IndexOf('\n');
                    var line = req.FileData[..charloc].Split(' ');
                    req.Url = line[1].Trim();
                    req.RequestType = (RequestMethod)Enum.Parse(typeof(RequestMethod), item);
                    req.FileData = req.FileData[charloc..];
                }
            }
            return req;

        }

        public static Request ParseHeader(this Request req)
        {
            req.FileData = req.FileData.TrimStart();

            if (req.FileData.StartsWith(Tokens.Header))
            {
                req.FileData = req.FileData[Tokens.Header.Length..].Trim(' ', ':').Trim();
                Stack<char> brack = new();
                bool isKey = false;
                bool isQuote = false;
                brack.Push(req.FileData[0]);

                StringBuilder key = new();
                StringBuilder value = new();

                int i = 0;
                while (brack.Count > 0 && req.FileData.Length >= i)
                {
                    i++;
                    if (req.FileData[i] == '{')
                    {
                        brack.Push(req.FileData[i]);
                        continue;
                    }

                    if (req.FileData[i] == '}')
                    {
                        brack.Pop();
                        continue;
                    }

                    if (req.FileData[i] == '"')
                    {
                        isQuote = !isQuote;
                        isKey = isQuote ^ isKey;
                        if (key.Length > 0 && value.Length > 0)
                        {
                            req.Headers.Add(key.ToString(), value.ToString());
                            key = key.Clear();
                            value = value.Clear();
                        }
                        continue;
                    }

                    if (isQuote && isKey)
                    {
                        key.Append(req.FileData[i]);
                        continue;
                    }

                    if (isQuote && !isKey)
                    {
                        value.Append(req.FileData[i]);
                        continue;
                    }

                }
                req.FileData = req.FileData[++i..];

            }
            return req;

        }

        public static Request ParseBody(this Request req)
        {
            req.FileData = req.FileData.TrimStart();

            if (req.FileData.StartsWith(Tokens.Body))
            {
                req.FileData = req.FileData[Tokens.Body.Length..].Trim(' ', ':').Trim();
                StringBuilder body = new();
                Stack<char> brack = new();
                brack.Push(req.FileData[0]);
                body.Append(req.FileData[0]);
                int i = 0;
                while (brack.Count > 0 && req.FileData.Length >= i)
                {
                    i++;

                    if (req.FileData[i] == '{')
                    {
                        brack.Push(req.FileData[i]);
                    }

                    if (req.FileData[i] == '}')
                    {
                        brack.Pop();
                    }

                    body.Append(req.FileData[i]);

                }

                req.Body = body.ToString();
                req.FileData = req.FileData[++i..];

            }

            return req;
        }

        public static Request ReplaceVar(this Request req, Dictionary<string, string> ver)
        {

            var regexPattern = "[*][^\\s]+[*]";
            var matches = Regex.Matches(req.Url, regexPattern);
            var variables = matches.Select(x => x.Value.Trim('*')).ToList();

            foreach (var key in ver.Keys)
            {
                if (!req.Var.ContainsKey(key)) req.Var.TryAdd(key, ver[key]);
            }

            foreach (var v in variables)
            {
                req.Url = req.Url.Replace($"*{v}*", req.Var[v]);
            }

            if (!string.IsNullOrEmpty(req.Body))
            {
                matches = Regex.Matches(req.Body, regexPattern);
                variables = matches.Select(x => x.Value.Trim('*')).ToList();
                foreach (var v in variables)
                {
                    req.Body = req.Body.Replace($"*{v}*", req.Var[v]);
                }
            }

            foreach (var key in req.Headers.Keys)
            {
                matches = Regex.Matches(req.Headers[key], regexPattern);
                variables = matches.Select(x => x.Value.Trim('*')).ToList();

                foreach (var v in variables)
                {
                    req.Headers[key] = req.Headers[key].Replace($"*{v}*", req.Var[v]);
                }

            }

            return req;
        }

        public static string FilterComments(this string fileData)
        {

            for (int i = 0; i < fileData.Length - 1; i++)
            {

                if (fileData[i] == '-' && fileData[i + 1] == '-')
                {
                    int charloc = i;

                    for (int j = i + 1; j < fileData.Length; j++)
                    {

                        if (fileData[j] == '\n')
                        {
                            charloc = j;
                            break;
                        }
                    }
                    fileData = fileData.Remove(i, charloc - i);
                }
            }

            return fileData;
        }

    }
}
