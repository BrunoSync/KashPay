using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Enums;

namespace KashPay.Application.Common.OneOf.Errors
{
    public record InvalidRefreshTokenError() : AppError("Invalid refresh token", ErrorsType.UnauthorizedError, nameof(InvalidRefreshTokenError));
}