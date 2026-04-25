using KashPay.Application.Extension;
using KashPay.Infrastructure.Extension;
using Scalar.AspNetCore;

// DotNetEnv
DotNetEnv.Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();