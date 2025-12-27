using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

public class Currency : BaseDomainEntity<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }

    private Currency(string code, string name, string symbol)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
    }

    public static Currency Create(string code, string name, string symbol)
    {
        var validator = new CurrencyValidator();

        var objectToValidate = new Currency(code, name, symbol);

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new CurrencyNotValidException("Currency is not valid");
        validationResult.Errors.ForEach(er => exception.ValidationErrors.Add(er.ErrorMessage));
        throw exception;
    }
}