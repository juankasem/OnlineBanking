using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Application.Contracts.Infrastructure
{
    public interface IAppUserAccessor
    {
        string GetUsername();
    }
}