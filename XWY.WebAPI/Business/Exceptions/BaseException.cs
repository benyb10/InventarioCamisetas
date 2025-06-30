using System.Net;

namespace XWY.WebAPI.Business.Exceptions
{
    public abstract class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ErrorCode { get; }
        public object? Details { get; }

        protected BaseException(string message, HttpStatusCode statusCode, string errorCode, object? details = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Details = details;
        }

        protected BaseException(string message, Exception innerException, HttpStatusCode statusCode, string errorCode, object? details = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Details = details;
        }
    }
}
