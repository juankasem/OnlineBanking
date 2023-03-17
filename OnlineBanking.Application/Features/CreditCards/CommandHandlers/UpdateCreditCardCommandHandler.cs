using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.CreditCards.Commands;
using OnlineBanking.Application.Features.CreditCards.Messages;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.CreditCards.CommandHandlers;

public class UpdateCreditCardCommandHandler : IRequestHandler<UpdateCreditCardCommand, ApiResult<Unit>>
{

    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IAppUserAccessor _appUserAccessor;

    public UpdateCreditCardCommandHandler(IUnitOfWork uow, IMapper mapper, IAppUserAccessor appUserAccessor)
    {
        _uow = uow;
        _mapper = mapper;
        _appUserAccessor = appUserAccessor;
    }

    public async Task<ApiResult<Unit>> Handle(UpdateCreditCardCommand request, CancellationToken cancellationToken)
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

            var creditCard = CreateCreditCard(request.CreditCardId, request.CreditCardNo, request.CustomerNo, request.ValidTo,
                                            request.SecurityCode, request.BankAccountId);
                        
            bankAccount.UpdateCreditCard(request.CreditCardId, creditCard);

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

        private CreditCard CreateCreditCard( Guid creditCardId, string creditCardNo, string	customerNo, 
                                    DateTime validTo, int securityCode, Guid bankAccountId) =>
        CreditCard.Create(creditCardNo, customerNo, validTo, securityCode, bankAccountId, creditCardId);
}
