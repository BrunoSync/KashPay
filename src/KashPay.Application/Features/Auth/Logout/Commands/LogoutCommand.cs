using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace KashPay.Application.Features.Auth.Logout.Commands
{
    public record LogoutCommand 
    (
        Guid UserId
    ) : IRequest;
}