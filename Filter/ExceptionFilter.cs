using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace be.Filter;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is UnauthorizedAccessException ex)
        {
            context.Result = new UnauthorizedObjectResult(new { message = ex.Message });
            
            context.ExceptionHandled = true;
        }
    }
}