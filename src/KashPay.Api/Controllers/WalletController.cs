using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KashPay.Application.DTOs;
using KashPay.Application.Features.Transaction.Queries.GetTransactions;
using KashPay.Application.Features.Wallet.Commands.Deposit;
using KashPay.Application.Features.Wallet.Commands.Transfer;
using KashPay.Application.Features.Wallet.Commands.Withdraw;
using KashPay.Application.Features.Wallet.Queries.GetBalance;
using KashPay.Domain.Entities;
using KashPay.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;

namespace KashPay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        // Mediator
        private readonly IMediator _mediator;

        public WalletController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBalance(CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var query = new GetBalanceQuery(userId);

            var result = await _mediator.Send(query, ct);

            return result.Match(
                success => Ok(success),
                error => error.type switch
                {
                    ErrorsType.NotFoundError => NotFound(error),
                    _ => StatusCode(500, error)
                }
            );
        }

        [HttpPost("deposit")]
        [EnableRateLimiting("transaction")]
        [Authorize]
        public async Task<IActionResult> Deposit(WalletAmountRequestDto request, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var command = new DepositCommand(userId, request.Amount);

            var result = await _mediator.Send(command, ct);

            return result.Match(
                success => Ok(success),
                error => error.type switch
                {
                    ErrorsType.NotFoundError => NotFound(error),
                    _ => StatusCode(500, error)
                }
            );
        }
        
        [HttpPost("withdraw")]
        [EnableRateLimiting("transaction")]
        [Authorize]
        public async Task<IActionResult> Withdraw(WalletAmountRequestDto request, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var command = new WithdrawCommand(userId, request.Amount);

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

        [HttpPost("transfer")]
        [EnableRateLimiting("transaction")]
        [Authorize]
        public async Task<IActionResult> Transfer(TransferRequestDto request, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var command = new TransferCommand(
                userId,
                request.AccountNumber,
                request.Amount
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

        [HttpGet("transactions")]
        [Authorize]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int pageSize,
            [FromQuery] DateTime? cursorTimestamp,
            [FromQuery] Guid? cursorId,
            CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var cursor = (cursorTimestamp, cursorId ?? Guid.Empty);
            var query = new GetTransactionsQuery(userId, pageSize, cursor);

            var result = await _mediator.Send(query, ct);

            return result.Match(
                success => Ok(success),
                error => error.type switch
                {
                    ErrorsType.NotFoundError => NotFound(error),
                    _ => StatusCode(500, error)
                }
            );
        }
    }
}