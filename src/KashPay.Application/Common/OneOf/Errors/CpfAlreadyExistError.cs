using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Enums;

namespace KashPay.Application.Common.OneOf.Errors
{
    public record CpfAlreadyExistError() : AppError("Cpf already exist", ErrorsType.ConflictError, nameof(CpfAlreadyExistError));
}