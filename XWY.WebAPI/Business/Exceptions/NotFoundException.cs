using System.Net;

namespace XWY.WebAPI.Business.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string resource)
            : base($"{resource} no encontrado", HttpStatusCode.NotFound, "NOT_FOUND")
        {
        }

        public NotFoundException(string resource, object identifier)
            : base($"{resource} con identificador '{identifier}' no encontrado", HttpStatusCode.NotFound, "NOT_FOUND")
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException, HttpStatusCode.NotFound, "NOT_FOUND")
        {
        }
    }
}
