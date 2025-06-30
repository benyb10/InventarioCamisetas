using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.WebAPI.Filters
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public AuthorizationFilter(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles ?? new string[0];
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!IsAuthenticated(context.HttpContext))
            {
                context.Result = new UnauthorizedObjectResult(new ResponseDto<object>
                {
                    Success = false,
                    Message = "Token de autenticación requerido",
                    Errors = new List<string> { "UNAUTHORIZED" }
                });
                return;
            }

            if (_allowedRoles.Any() && !HasRequiredRole(context.HttpContext))
            {
                context.Result = new ObjectResult(new ResponseDto<object>
                {
                    Success = false,
                    Message = "No tiene permisos para realizar esta acción",
                    Errors = new List<string> { "FORBIDDEN" }
                })
                {
                    StatusCode = 403
                };
                return;
            }
        }

        private bool IsAuthenticated(HttpContext context)
        {
            return context.User?.Identity?.IsAuthenticated == true;
        }

        private bool HasRequiredRole(HttpContext context)
        {
            if (!_allowedRoles.Any())
                return true;

            var userRole = context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userRole))
                return false;

            return _allowedRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase);
        }
    }

    public class RequireAuthenticationAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new AuthorizationFilter();
        }
    }

    public class RequireRoleAttribute : Attribute, IFilterFactory
    {
        private readonly string[] _roles;

        public RequireRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new AuthorizationFilter(_roles);
        }
    }

    public class RequireAdminAttribute : RequireRoleAttribute
    {
        public RequireAdminAttribute() : base("Administrador")
        {
        }
    }

    public class RequireOperatorAttribute : RequireRoleAttribute
    {
        public RequireOperatorAttribute() : base("Operador", "Administrador")
        {
        }
    }
}
