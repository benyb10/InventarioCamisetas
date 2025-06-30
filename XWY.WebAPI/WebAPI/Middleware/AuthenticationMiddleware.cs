using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using XWY.WebAPI.Business.Services;

namespace XWY.WebAPI.WebAPI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            var token = ExtractTokenFromRequest(context);

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var validationResult = await authService.ValidateTokenAsync(token);

                    if (validationResult.Success)
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtToken = tokenHandler.ReadJwtToken(token);

                        var claims = new List<Claim>();
                        foreach (var claim in jwtToken.Claims)
                        {
                            claims.Add(new Claim(claim.Type, claim.Value));
                        }

                        var identity = new ClaimsIdentity(claims, "jwt");
                        context.User = new ClaimsPrincipal(identity);

                        _logger.LogInformation("Usuario autenticado: {UserId}",
                            context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    }
                    else
                    {
                        _logger.LogWarning("Token inválido: {Token}", token);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al validar token: {Message}", ex.Message);
                }
            }

            await _next(context);
        }

        private string ExtractTokenFromRequest(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                return authorizationHeader.Substring("Bearer ".Length).Trim();
            }

            var tokenFromQuery = context.Request.Query["token"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tokenFromQuery))
            {
                return tokenFromQuery;
            }

            var tokenFromCookie = context.Request.Cookies["auth_token"];
            if (!string.IsNullOrEmpty(tokenFromCookie))
            {
                return tokenFromCookie;
            }

            return null;
        }
    }
}
