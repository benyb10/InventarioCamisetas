using Newtonsoft.Json;
using System.Net;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Business.Exceptions;

namespace XWY.WebAPI.WebAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ResponseDto<object>();

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Success = false;
                    response.Message = "Error de validación";
                    response.Errors = validationEx.ValidationErrors;
                    break;

                case BaseException baseEx:
                    context.Response.StatusCode = (int)baseEx.StatusCode;
                    response.Success = false;
                    response.Message = baseEx.Message;
                    response.Errors = new List<string> { baseEx.ErrorCode };
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Success = false;
                    response.Message = "No autorizado";
                    response.Errors = new List<string> { "UNAUTHORIZED" };
                    break;

                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Success = false;
                    response.Message = argEx.Message;
                    response.Errors = new List<string> { "INVALID_ARGUMENT" };
                    break;

                case InvalidOperationException invalidOpEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Success = false;
                    response.Message = invalidOpEx.Message;
                    response.Errors = new List<string> { "INVALID_OPERATION" };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Success = false;
                    response.Message = "Error interno del servidor";
                    response.Errors = new List<string> { "INTERNAL_SERVER_ERROR" };
                    break;
            }

            var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
