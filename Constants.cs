namespace HReq
{
    internal class Constants
    {
    }

    public static class Tokens
    {
        public static readonly string Header = "HEADERS";
        public static readonly string Body = "BODY";
        public static readonly string ContentType = "Content-Type";
    }

    public enum RequestMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
