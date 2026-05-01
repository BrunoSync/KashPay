using KashPay.Api.Configuration;
using KashPay.Api.Global;
using KashPay.Application.Extension;
using KashPay.Infrastructure.Data;
using KashPay.Infrastructure.Extension;
using Microsoft.EntityFrameworkCore;
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();