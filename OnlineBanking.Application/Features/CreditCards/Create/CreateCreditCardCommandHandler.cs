using AutoMapper;
using OnlineBanking.Application.Features.CreditCards.Messages;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.CreditCards.Create;

public class CreateCreditCardCommandHandler : IRequestHandler<CreateCreditCardCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IAppUserAccessor _appUserAccessor;

    public CreateCreditCardCommandHandler(IUnitOfWork uow, IMapper mapper, IAppUserAccessor appUserAccessor)
    {
        _uow = uow;
        _mapper = mapper;
        _appUserAccessor = appUserAccessor;
    }
    public async Task<ApiResult<Unit>> Handle(CreateCreditCardCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        try
        {
            var bankAccount = await _uow.BankAccounts.GetByIdAsync(request.BankAccountId);

            if (bankAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, "Id", request.BankAccountId));

                return result;
            }

            if (!bankAccount.BankAccountOwners.Any(b => b.Customer.AppUserId == loggedInAppUser.Id))
            {
                result.AddError(ErrorCode.UnAuthorizedOperation, CreditCardsErrorMessages.CreateNotAuthorized);

                return result;
            }

            var creditCard = CreateCreditCard(request.CreditCardNo, request.CustomerNo, request.ValidTo,
                                            request.SecurityCode, request.BankAccountId);

            bankAccount.AddCreditCard(creditCard);

            _uow.BankAccounts.Update(bankAccount);
            await _uow.SaveAsync();

            return result;
        }
        catch (CreditCardNotValidException e)
        {
            e.ValidationErrors.ForEach(er => result.AddError(ErrorCode.ValidationError, er));
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    private CreditCard CreateCreditCard(string creditCardNo, string customerNo,
                                    DateTime validTo, int securityCode, Guid bankAccountId) =>
        CreditCard.Create(creditCardNo, customerNo, validTo, securityCode, bankAccountId);
}
