using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace frontend.Models;

public class UnauthorizedRedirectFilter : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is HttpRequestException httpEx &&
            httpEx.StatusCode == HttpStatusCode.Unauthorized)
        {
            context.Result = new RedirectResult("/Auth/Login");
            context.ExceptionHandled = true;
        }
        return Task.CompletedTask;
    }
}
