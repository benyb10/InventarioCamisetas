using System.Net;

namespace XWY.WebAPI.Business.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException()
            : base("No autorizado", HttpStatusCode.Unauthorized, "UNAUTHORIZED")
        {
        }

        public UnauthorizedException(string message)
            : base(message, HttpStatusCode.Unauthorized, "UNAUTHORIZED")
        {
        }

        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException, HttpStatusCode.Unauthorized, "UNAUTHORIZED")
        {
        }
    }
}
