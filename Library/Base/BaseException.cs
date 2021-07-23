using System;
using System.Net;

namespace Library.Base
{
    public abstract class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public string? InternalMessage { get; set; }

        public BaseException(HttpStatusCode statusCode, string message) : base(message) =>
            StatusCode = statusCode;

        public BaseException(HttpStatusCode statusCode, string message, string internalMessage) : base(message)
        {
            StatusCode = statusCode;
            InternalMessage = internalMessage;
        }
    }

    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }

        public BadRequestException(string message, string internalMessage) : base(HttpStatusCode.BadRequest, message, internalMessage) { }
    }

    public class InternalServerException : BaseException
    {
        public InternalServerException(string message) : base(HttpStatusCode.InternalServerError, message) { }

        public InternalServerException(string message, string internalMessage) : base(HttpStatusCode.InternalServerError, message, internalMessage) { }
    }

    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message) { }
    }
}