using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineBanking.Core.Domain.Entities;

namespace OnlineBanking.Application.Contracts.Infrastructure
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}