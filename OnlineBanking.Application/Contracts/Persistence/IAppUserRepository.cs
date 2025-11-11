using OnlineBanking.Core.Domain.Entities;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface IAppUserRepository : IGenericRepository<AppUser>
{
    Task<AppUser> GetAppUser(string userName);
}
