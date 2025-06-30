using System.Net;

namespace XWY.WebAPI.Business.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException()
            : base("Acceso denegado", HttpStatusCode.Forbidden, "FORBIDDEN")
        {
        }

        public ForbiddenException(string message)
            : base(message, HttpStatusCode.Forbidden, "FORBIDDEN")
        {
        }

        public ForbiddenException(string message, Exception innerException)
            : base(message, innerException, HttpStatusCode.Forbidden, "FORBIDDEN")
        {
        }
    }
}
