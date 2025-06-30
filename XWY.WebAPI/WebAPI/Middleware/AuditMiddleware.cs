using System.Security.Claims;
using System.Text;
using XWY.WebAPI.Business.Services;

namespace XWY.WebAPI.WebAPI.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditoriaService auditoriaService)
        {
            if (ShouldAuditRequest(context))
            {
                var requestBody = await CaptureRequestBodyAsync(context);
                var startTime = DateTime.UtcNow;

                var originalResponseBodyStream = context.Response.Body;
                using var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                try
                {
                    await _next(context);

                    var endTime = DateTime.UtcNow;
                    var duration = endTime - startTime;

                    await LogAuditAsync(context, auditoriaService, requestBody, duration);

                    await responseBodyStream.CopyToAsync(originalResponseBodyStream);
                }
                catch (Exception ex)
                {
                    var endTime = DateTime.UtcNow;
                    var duration = endTime - startTime;

                    await LogErrorAuditAsync(context, auditoriaService, requestBody, ex, duration);

                    await responseBodyStream.CopyToAsync(originalResponseBodyStream);
                    throw;
                }
                finally
                {
                    context.Response.Body = originalResponseBodyStream;
                }
            }
            else
            {
                await _next(context);
            }
        }

        private bool ShouldAuditRequest(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            var method = context.Request.Method.ToUpper();

            var auditablePaths = new[]
            {
                "/api/auth/login",
                "/api/auth/register",
                "/api/usuario",
                "/api/articulo",
                "/api/prestamo",
                "/api/reporte",
                "/api/catalogo"
            };

            var auditableMethods = new[] { "POST", "PUT", "DELETE" };

            return auditablePaths.Any(p => path?.StartsWith(p) == true) ||
                   (auditableMethods.Contains(method) && path?.StartsWith("/api/") == true);
        }

        private async Task<string> CaptureRequestBodyAsync(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();

                using var reader = new StreamReader(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true);

                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                return body;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al capturar el cuerpo de la petición");
                return string.Empty;
            }
        }

        private async Task LogAuditAsync(HttpContext context, IAuditoriaService auditoriaService, string requestBody, TimeSpan duration)
        {
            try
            {
                var userId = GetUserIdFromContext(context);
                var action = DetermineAction(context);
                var table = DetermineTable(context);
                var ipAddress = GetClientIpAddress(context);
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                var auditData = new
                {
                    Method = context.Request.Method,
                    Path = context.Request.Path.Value,
                    QueryString = context.Request.QueryString.Value,
                    RequestBody = TruncateString(requestBody, 1000),
                    StatusCode = context.Response.StatusCode,
                    Duration = duration.TotalMilliseconds,
                    Timestamp = DateTime.UtcNow
                };

                await auditoriaService.LogAccionAsync(
                    userId,
                    action,
                    table,
                    null,
                    null,
                    auditData,
                    ipAddress,
                    userAgent
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar auditoría: {Message}", ex.Message);
            }
        }

        private async Task LogErrorAuditAsync(HttpContext context, IAuditoriaService auditoriaService, string requestBody, Exception exception, TimeSpan duration)
        {
            try
            {
                var userId = GetUserIdFromContext(context);
                var ipAddress = GetClientIpAddress(context);
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                var errorData = new
                {
                    Method = context.Request.Method,
                    Path = context.Request.Path.Value,
                    QueryString = context.Request.QueryString.Value,
                    RequestBody = TruncateString(requestBody, 1000),
                    ErrorMessage = exception.Message,
                    ErrorType = exception.GetType().Name,
                    Duration = duration.TotalMilliseconds,
                    Timestamp = DateTime.UtcNow
                };

                await auditoriaService.LogAccionAsync(
                    userId,
                    "ERROR",
                    "System",
                    null,
                    null,
                    errorData,
                    ipAddress,
                    userAgent
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar auditoría de error: {Message}", ex.Message);
            }
        }

        private int? GetUserIdFromContext(HttpContext context)
        {
            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }

        private string DetermineAction(HttpContext context)
        {
            var method = context.Request.Method.ToUpper();
            var path = context.Request.Path.Value?.ToLower();

            if (path?.Contains("/login") == true) return "LOGIN";
            if (path?.Contains("/register") == true) return "REGISTER";
            if (path?.Contains("/logout") == true) return "LOGOUT";
            if (path?.Contains("/reporte") == true) return "EXPORT";

            return method switch
            {
                "GET" => "READ",
                "POST" => "CREATE",
                "PUT" => "UPDATE",
                "DELETE" => "DELETE",
                _ => "UNKNOWN"
            };
        }

        private string DetermineTable(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            if (path?.Contains("/usuario") == true) return "Usuarios";
            if (path?.Contains("/articulo") == true) return "Articulos";
            if (path?.Contains("/prestamo") == true) return "Prestamos";
            if (path?.Contains("/catalogo") == true) return "Catalogos";
            if (path?.Contains("/reporte") == true) return "Reportes";
            if (path?.Contains("/auditoria") == true) return "AuditoriaLog";
            if (path?.Contains("/auth") == true) return "Auth";

            return "System";
        }

        private string GetClientIpAddress(HttpContext context)
        {
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Connection.RemoteIpAddress?.ToString();
            }

            return ipAddress ?? "Unknown";
        }

        private string TruncateString(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
            {
                return input;
            }

            return input.Substring(0, maxLength) + "...";
        }
    }
}
