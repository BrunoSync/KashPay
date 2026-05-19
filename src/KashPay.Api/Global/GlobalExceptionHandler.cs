using System;
using System.Collections.Generic;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;

namespace KashPay.Api.Global
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken ct)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            var (statusCode, message) = exception switch
            {
                ValidationException ex => (
                    StatusCodes.Status400BadRequest,
                    ex.Errors.Select(e => e.ErrorMessage).ToArray() as IEnumerable<string>
                ),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    new[] { "An unexpected error occurred" } as IEnumerable<string>
                )
            };

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = statusCode,
                Errors = message
            }, ct);

            return true;
        }
    }
}