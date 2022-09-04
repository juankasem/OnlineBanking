using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.Branch.Commands
{
    public class CreateBranchCommand : IRequest<ApiResult<Unit>>
    {
        
    }
}