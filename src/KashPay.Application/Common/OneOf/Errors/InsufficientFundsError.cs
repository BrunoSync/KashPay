using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Enums;

namespace KashPay.Application.Common.OneOf.Errors
{
    public sealed record InsufficientFundsError() : AppError("Insufficient Funds", ErrorsType.BusinessError, nameof(InsufficientFundsError));
}