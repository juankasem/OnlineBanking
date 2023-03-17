using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Application.Models.CreditCard.Responses;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Mappings.CreditCards;

public class CreditCardsMapper : ICreditCardsMapper
{
    public CreditCardDto MapToDto(CreditCard creditCard)
    {
        return new CreditCardDto(CreateCreditCardHolder(creditCard.BankAccount, creditCard.CustomerNo), creditCard.CreditCardNo,
                                                        creditCard.CustomerNo, creditCard.ValidTo, creditCard.SecurityCode);
    }

    public CreditCardDetailsResponse MapToResponseModel(CreditCard creditCard)
    {
        return new CreditCardDetailsResponse(CreateCreditCardHolder(creditCard.BankAccount, creditCard.CustomerNo), creditCard.CreditCardNo,
                                                                    creditCard.CustomerNo, creditCard.ValidTo, creditCard.SecurityCode,
                                                                    creditCard.BankAccount.Balance, creditCard.IsActive);
    }

    private string CreateCreditCardHolder(BankAccount bankAccount, string cutomerNo)
    {
        var holder = bankAccount.BankAccountOwners.FirstOrDefault(b => b.Customer.CustomerNo == cutomerNo);

        if (holder is not null)
        {
            return holder.Customer.FirstName + ' ' + holder.Customer.LastName;
        }

        return string.Empty;
    }
}