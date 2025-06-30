using System.Net;

namespace XWY.WebAPI.Business.Exceptions
{
    public class ValidationException : BaseException
    {
        public List<string> ValidationErrors { get; }

        public ValidationException(string message)
            : base(message, HttpStatusCode.BadRequest, "VALIDATION_ERROR")
        {
            ValidationErrors = new List<string> { message };
        }

        public ValidationException(List<string> errors)
            : base("Error de validación", HttpStatusCode.BadRequest, "VALIDATION_ERROR", errors)
        {
            ValidationErrors = errors;
        }

        public ValidationException(string field, string message)
            : base($"Error en el campo {field}: {message}", HttpStatusCode.BadRequest, "VALIDATION_ERROR")
        {
            ValidationErrors = new List<string> { $"{field}: {message}" };
        }
    }
}
