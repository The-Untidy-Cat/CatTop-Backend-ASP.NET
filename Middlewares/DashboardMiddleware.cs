using asp.net.Data;
using asp.net.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using System.Net;
using System.Threading.Tasks;

namespace asp.net.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class DashboardMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthSetting _authSettings;


        public DashboardMiddleware(RequestDelegate next, IOptions<AuthSetting> options)
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
                return;
            }
            var employee = await context.Employees.Where(c => c.User.Username == user).FirstOrDefaultAsync();
            Console.WriteLine(employee.ToJson());
            if (employee == null)
            {
                await ReturnErrorResponse(httpContext, HttpStatusCode.Forbidden);
                return;
            }
            httpContext.Items["user"] = employee;
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
            return;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class DashboardMiddlewareExtensions
    {
        public static IApplicationBuilder UseDashboardMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DashboardMiddleware>();
        }
    }
}
