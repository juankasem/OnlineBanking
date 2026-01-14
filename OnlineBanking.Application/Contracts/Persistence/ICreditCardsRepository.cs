
namespace OnlineBanking.Application.Contracts.Persistence;

public interface ICreditCardsRepository : IGenericRepository<CreditCard>
{
    public Task<IReadOnlyList<CreditCard>> GetCustomerCreditCardsAsync(string customerNo);
}
