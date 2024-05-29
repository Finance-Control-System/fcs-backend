using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentValidation;
using Domain.Exceptions;

namespace Presentation.Filters;

internal class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order { get; set; } = int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception == null)
            return;

        if (context.Exception is HttpResponseException exception)
        {
            context.Result = new ObjectResult(exception.Value)
            {
                StatusCode = exception.Status,
            };
            context.ExceptionHandled = true;
        }
        else if (context.Exception is ValidationException validationException)
        {
            context.Result = new ObjectResult(string.Join(Environment.NewLine,
                validationException.Errors.Select(e => e.ErrorMessage)))
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
            };
            context.ExceptionHandled = true;
        }
        else
        {
            context.Result = new ObjectResult(context.Exception.Message)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
            context.ExceptionHandled = true;
        }
    }
}
