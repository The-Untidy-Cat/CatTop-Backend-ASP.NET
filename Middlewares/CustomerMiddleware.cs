using asp.net.Data;
using asp.net.Models;
using asp.net.Services;
using Microsoft.Extensions.Options;
using System.Net;

namespace asp.net.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CustomerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthSetting _authSettings;


        public CustomerMiddleware(RequestDelegate next, IOptions<AuthSetting> options)
        {
            _next = next;
            _authSettings = options.Value;
        }

        public async Task Invoke(HttpContext httpContext, DbCtx context)
        {
            var user = httpContext.Items["user"];
            if (user == null)
            {
                await ReturnErrorResponse(httpContext, HttpStatusCode.Unauthorized);
            }
            var customer = context.Customers.Where(c => c.User.Username == user).FirstOrDefault();
            if (customer == null)
            {
                await ReturnErrorResponse(httpContext, HttpStatusCode.Forbidden);
            }
            httpContext.Items["user"] = customer;   
            await _next(httpContext);
        }
        private async Task ReturnErrorResponse(HttpContext context, HttpStatusCode httpStatusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpStatusCode;
            context.Response.WriteAsJsonAsync(new
            {
                code = (int)httpStatusCode,
                message = "You are not allowed to access this resource"
            });
            await context.Response.CompleteAsync();
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomerMiddleware>();
        }
    }
}
