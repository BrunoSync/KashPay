using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Enums;

namespace KashPay.Application.Common.OneOf.Errors
{
    public record RefreshTokenNotFoundError() : AppError("Refresh token not found", ErrorsType.NotFoundError, nameof(RefreshTokenNotFoundError));
}