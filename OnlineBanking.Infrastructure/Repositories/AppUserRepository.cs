using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Entities;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class AppUserRepository : GenericRepository<AppUser>, IAppUserRepository
{
    public AppUserRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<AppUser> GetAppUser(string userName) =>
      await _dbContext.AppUsers.FirstOrDefaultAsync(a => a.UserName == userName);
    
}
