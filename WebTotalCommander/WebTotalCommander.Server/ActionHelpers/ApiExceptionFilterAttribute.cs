using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebTotalCommander.Core.Errors;

namespace WebTotalCommander.Server.ActionHelpers;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext actionExecutedContext)
    {
        var code = 500;
        if (actionExecutedContext.Exception is EntryNotFoundException)
        {
            code = 404; // HTTP for Not Found
        }
        if (actionExecutedContext.Exception is AlreadeExsistException)
        {
            code = 409;
        }
        if (actionExecutedContext.Exception is ParameterInvalidException)
        {
            code = 422;
        }


        actionExecutedContext.HttpContext.Response.StatusCode = code;
        actionExecutedContext.Result = new JsonResult(new
        {
            error = actionExecutedContext.Exception.Message
        });
    }
}
