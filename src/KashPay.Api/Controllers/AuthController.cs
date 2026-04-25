using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Features.Auth.Login.Queries;
using KashPay.Application.Features.Auth.Login.Register.Commands;
using KashPay.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KashPay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            return result.Match(
                success => Ok(success),
                error => error.type switch
                {
                    ErrorsType.UnauthorizedError => Unauthorized(error),
                    _ => StatusCode(500, error)
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            return result.Match(
                success => Created("/me", success),
                error => error.type switch
                {
                    ErrorsType.ConflictError => Conflict(error),
                    _ => StatusCode(500, error)
                }
            );
        }
    }
}