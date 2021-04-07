namespace Api.Library.Error.Contract
{
    public class ErrorRequestContract
    {
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