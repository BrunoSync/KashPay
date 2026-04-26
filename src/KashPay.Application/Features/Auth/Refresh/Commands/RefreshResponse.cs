using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Application.Features.Auth.Refresh.Commands
{
    public record RefreshResponse
    (
        string AccessToken, 
        string RefreshToken
    );
}