using System.Net;

namespace XWY.WebAPI.Business.Exceptions
{
    public class ConflictException : BaseException
    {
        public ConflictException(string message)
            : base(message, HttpStatusCode.Conflict, "CONFLICT")
        {
        }

        public ConflictException(string resource, string value)
            : base($"Ya existe un {resource} con el valor '{value}'", HttpStatusCode.Conflict, "CONFLICT")
        {
        }

        public ConflictException(string message, Exception innerException)
            : base(message, innerException, HttpStatusCode.Conflict, "CONFLICT")
        {
        }
    }
}
