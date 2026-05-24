using System.Threading.RateLimiting;
using KashPay.Api.Configuration;
using KashPay.Api.Global;
using KashPay.Application.Extension;
using KashPay.Infrastructure.Data;
using KashPay.Infrastructure.Extension;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

// DotNetEnv
DotNetEnv.Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

// Global Exception
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("login", o =>
    {
        o.PermitLimit = 5;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("register", o =>
    {
        o.PermitLimit = 10;
        o.Window = TimeSpan.FromMinutes(5);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("forgotpassword", o =>
    {
      o.PermitLimit = 5;
      o.Window = TimeSpan.FromMinutes(30);
      o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
      o.QueueLimit = 0;  
    });

    options.AddFixedWindowLimiter("resetpassword", o =>
    {
      o.PermitLimit = 5;
      o.Window = TimeSpan.FromMinutes(15);
      o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
      o.QueueLimit = 0;  
    });

    options.AddFixedWindowLimiter("authenticated", o =>
    {
      o.PermitLimit = 10;
      o.Window = TimeSpan.FromMinutes(10);
      o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
      o.QueueLimit = 0;  
    });

    options.AddFixedWindowLimiter("transaction", o =>
    {
        o.PermitLimit = 10;
        o.Window = TimeSpan.FromMinutes(10);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Controllers
builder.Services.AddControllers();

// Infra
builder.Services.AddInfrastructure(builder.Configuration);

// Application
builder.Services.AddApplication();

builder.Services.AddOpenApi(opts =>
{
    opts.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();