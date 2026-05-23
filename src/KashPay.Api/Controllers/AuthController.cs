using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KashPay.Application.DTOs;
using KashPay.Application.Features.Auth.ForgetPassword.Commands;
using KashPay.Application.Features.Auth.ForgotPassword.Commands;
using KashPay.Application.Features.Auth.Login.Queries;
using KashPay.Application.Features.Auth.Login.Register.Commands;
using KashPay.Application.Features.Auth.Logout.Commands;
using KashPay.Application.Features.Auth.Refresh.Commands;
using KashPay.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace KashPay.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [EnableRateLimiting("login")]
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
        [EnableRateLimiting("register")]
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

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var command = new LogoutCommand(userId);
            await _mediator.Send(command, ct);
            return NoContent();
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> Refresh(RefreshCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            return result.Match(
                success => Ok(success),
                error => error.type switch
                {
                    ErrorsType.NotFoundError => NotFound(error),
                    ErrorsType.UnauthorizedError => Unauthorized(error),
                    _ => StatusCode(500, error)
                }
            );
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequest request, CancellationToken ct)
        {
            var command = new ResetPasswordCommand(
                request.Email,
                request.Code,
                request.NewPassword,
                request.ConfirmNewPassword
            );

            var result = await _mediator.Send(command, ct);

            return result.Match(
                success => Ok(success),
                error => error.type switch
                {
                    ErrorsType.NotFoundError => NotFound(error),
                    ErrorsType.BusinessError => BadRequest(error),
                    _ => StatusCode(500, error)
                }
            );
        }

        [HttpPost("forgetpassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            return Ok(result);
        }
    }
}