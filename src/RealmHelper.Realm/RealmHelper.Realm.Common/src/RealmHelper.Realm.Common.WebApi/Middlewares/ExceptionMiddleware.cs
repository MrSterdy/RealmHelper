using System.Net;

using Microsoft.AspNetCore.Http;

namespace RealmHelper.Realm.Common.WebApi.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (HttpRequestException e)
        {
            context.Response.StatusCode = e.StatusCode == HttpStatusCode.InternalServerError
                ? StatusCodes.Status400BadRequest
                : (int)e.StatusCode!;
        }
    }
}