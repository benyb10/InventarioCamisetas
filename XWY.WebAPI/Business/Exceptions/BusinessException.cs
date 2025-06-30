using System.Net;

namespace XWY.WebAPI.Business.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string message)
            : base(message, HttpStatusCode.BadRequest, "BUSINESS_ERROR")
        {
        }

        public BusinessException(string message, string errorCode)
            : base(message, HttpStatusCode.BadRequest, errorCode)
        {
        }

        public BusinessException(string message, object details)
            : base(message, HttpStatusCode.BadRequest, "BUSINESS_ERROR", details)
        {
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException, HttpStatusCode.BadRequest, "BUSINESS_ERROR")
        {
        }
    }
}
