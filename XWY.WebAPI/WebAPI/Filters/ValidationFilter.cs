using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.WebAPI.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = new List<string>();

                foreach (var modelError in context.ModelState)
                {
                    foreach (var error in modelError.Value.Errors)
                    {
                        var fieldName = modelError.Key;
                        var errorMessage = error.ErrorMessage;
                        errors.Add($"{fieldName}: {errorMessage}");
                    }
                }

                var response = new ResponseDto<object>
                {
                    Success = false,
                    Message = "Error de validación",
                    Errors = errors,
                    Data = null
                };

                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }

    public class ValidationFilterAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new ValidationFilter();
        }
    }
}
