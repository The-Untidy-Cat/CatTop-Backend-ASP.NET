using asp.net.Data;
using asp.net.Middlewares;
using asp.net.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var allowOrigins = builder.Configuration.GetSection("CorsSetting:AllowOrigins").Get<string[]>();

builder.Services.Configure<AuthSetting>(builder.Configuration.GetSection("AuthSetting"));
//builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
            builder => builder
                .WithOrigins(allowOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
});

builder.Services.AddDbContext<DbCtx>(options => options
    .UseMySql(connectionString,
    ServerVersion.AutoDetect(connectionString)
    ), ServiceLifetime.Scoped);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseRouting();
app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

// Bảo vệ tất cả các route bằng middleware UserMiddleware
app.UseUserMiddleware();
// Bảo vệ tất cả các route customer bằng middleware CustomerMiddleware
app.Map("/v1/customer", subApp =>
{
    subApp.UseCustomerMiddleware();
    subApp.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
});

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