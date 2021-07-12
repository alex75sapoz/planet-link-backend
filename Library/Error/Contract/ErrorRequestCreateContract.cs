namespace Library.Error.Contract
{
    public class ErrorRequestCreateContract
    {
        public ErrorRequestCreateContract(string method, string path, string query, int statusCodeId, string exceptionType, string exceptionMessage, string timezoneId)
        {
            Method = method;
            Path = path;
            Query = query;
            StatusCodeId = statusCodeId;
            ExceptionType = exceptionType;
            ExceptionMessage = exceptionMessage;
            TimezoneId = timezoneId;
        }

        public string Method { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public int StatusCodeId { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public int? UserSessionId { get; set; }
        public int? UserId { get; set; }
        public string TimezoneId { get; set; }
    }
}