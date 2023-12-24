using asp.net.Data;
using asp.net.Middlewares;
using asp.net.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.Configure<AuthSetting>(builder.Configuration.GetSection("AuthSetting"));
builder.Services.AddDbContext<DbCtx>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)), ServiceLifetime.Scoped);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseRouting();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

// Bảo vệ tất cả các route bằng middleware UserMiddleware
app.UseUserMiddleware();


// Bảo vệ tất cả các route customer bằng middleware CustomerMiddleware
//app.Map("/v1/customers", subApp =>
//{
//    subApp.UseCustomerMiddleware();
//    subApp.UseEndpoints(endpoints =>
//    {
//        endpoints.MapControllers();
//    });
//});

// Bảo vệ tất cả các route dashboard bằng middleware DashboardMiddleware
app.Map("/v1/dashboard", subApp =>
{
    subApp.UseDashboardMiddleware();
    subApp.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
});

app.Run();