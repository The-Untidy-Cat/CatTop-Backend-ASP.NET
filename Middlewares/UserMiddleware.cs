using asp.net.Controllers.Auth;
using asp.net.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace asp.net.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class UserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthSetting _authSettings;

        public UserMiddleware(RequestDelegate next, IOptions<AuthSetting> options)
        {
            _next = next;
            _authSettings = options.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var token = httpContext.Request.Cookies["token"];
            if (token == null)
            {
                await _next(httpContext);
            }
            var user = AuthService.ValidateToken(token, _authSettings);
            if (user == null)
            {
                await _next(httpContext);
            }
            httpContext.Items["user"] = user;
            await _next(httpContext);
        }
        private async Task ReturnErrorResponse(HttpContext context, HttpStatusCode httpStatusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpStatusCode;
            await context.Response.CompleteAsync();
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class UserMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserMiddleware>();
        }
    }
}
