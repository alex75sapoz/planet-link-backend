namespace Library.Application.Contract
{
    public class ApplicationErrorAuthenticationCreateContract
    {
        public ApplicationErrorAuthenticationCreateContract(string method, string path, string query, string exceptionType, string exceptionMessage)
        {
            Method = method;
            Path = path;
            Query = query;
            ExceptionType = exceptionType;
            ExceptionMessage = exceptionMessage;
        }

        public string Method { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public string? TimezoneId { get; set; }
        public string? UserTypeId { get; set; }
        public string? Token { get; set; }
        public string? Code { get; set; }
        public string? Subdomain { get; set; }
        public string? Page { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
    }
}