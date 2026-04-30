using KashPay.Api.Global;
using KashPay.Application.Extension;
using KashPay.Infrastructure.Extension;
using Scalar.AspNetCore;

// DotNetEnv
DotNetEnv.Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

// Global Exception
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Controllers
builder.Services.AddControllers();

// Infra
builder.Services.AddInfrastructure(builder.Configuration);

// Application
builder.Services.AddApplication();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();