using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Application.Features.Auth.Login.Queries
{
    public record LoginResponse
    (
        string AccessToken,
        string RefreshToken,
        DateTime LoginAt
    );
}