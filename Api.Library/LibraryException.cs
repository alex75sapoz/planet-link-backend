using System;
using System.Net;

namespace Api.Library
{
    public abstract class LibraryException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public string InternalMessage { get; set; }

        public LibraryException(HttpStatusCode statusCode, string message) : base(message) =>
            StatusCode = statusCode;

        public LibraryException(HttpStatusCode statusCode, string message, string internalMessage) : base(message)
        {
            StatusCode = statusCode;
            InternalMessage = internalMessage;
        }
    }

    public class BadRequestException : LibraryException
    {
        public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }

        public BadRequestException(string message, string internalMessage) : base(HttpStatusCode.BadRequest, message, internalMessage) { }
    }

    public class InternalServerException : LibraryException
    {
        public InternalServerException(string message) : base(HttpStatusCode.BadRequest, message) { }

        public InternalServerException(string message, string internalMessage) : base(HttpStatusCode.BadRequest, message, internalMessage) { }
    }
}