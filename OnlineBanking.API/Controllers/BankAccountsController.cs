using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Extensions;
using OnlineBanking.Application.Features.BankAccounts.Commands;
using OnlineBanking.Application.Features.BankAccounts.Queries;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Application.Models.BankAccount.Requests;
using OnlineBanking.Application.Models.BankAccount.Responses;
using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.FastTransaction.Requests;
using OnlineBanking.Core.Domain.Enums;


namespace OnlineBanking.API.Controllers;

[Authorize]
public class BankAccountsController : BaseApiController
{
    [HttpGet(ApiRoutes.BankAccounts.All)]
    [ProducesResponseType(typeof(PagedList<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAllBankAccounts([FromQuery] BankAccountParams bankAccountParams, CancellationToken cancellationToken = default)
    {
         var query = new GetAllBankAccountsRequest() 
        { 
            BankAccountParams = bankAccountParams 
        };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        var bankAccounts = result.Payload.Data;

        if (bankAccounts.Any())
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                                         result.Payload.TotalCount, result.Payload.TotalPages);
        }

        return Ok(bankAccounts);
    }

    [HttpGet(ApiRoutes.BankAccounts.GetByCustomerNo)]
    [ProducesResponseType(typeof(PagedList<BankAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBankAccountsByCustomerNo([FromRoute] string customerNo,
                                                                 [FromQuery] BankAccountParams bankAccountParams,
                                                                 [FromQuery] CashTransactionParams accountTransactionsParams,                                                
                                                                 CancellationToken cancellationToken = default)
    {
        var query = new GetBankAccountsByCustomerNoRequest() 
        { 
            CustomerNo = customerNo,
            BankAccountParams = bankAccountParams,
            AccountTransactionsParams = accountTransactionsParams 
        };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        var bankAccounts = result.Payload.Data;

        if (bankAccounts.Any())
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                                         result.Payload.TotalCount, result.Payload.TotalPages);
        }

        return Ok(bankAccounts);
    }

    [HttpGet(ApiRoutes.BankAccounts.GetByAccountNo)]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBankAccountByAccountNo([FromRoute] string accountNo,
                                                                [FromQuery] CashTransactionParams accountTransactionsParams,                                                
                                                                CancellationToken cancellationToken = default)
    {
        var query = new GetBankAccountByAccountNoRequest() 
        { 
            AccountNo = accountNo, 
            AccountTransactionsParams = accountTransactionsParams
        };

        return await HandleRequest(query, cancellationToken);
    }

    [HttpGet(ApiRoutes.BankAccounts.GetByIBAN)]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBankAccountByIBAN([FromRoute] string iban,
                                                          [FromQuery] CashTransactionParams accountTransactionParams,                                                
                                                           CancellationToken cancellationToken = default)
    {
        var query = new GetBankAccountWithTransactionsRequest()
        {
            IBAN = iban,
            AccountTransactionsParams = accountTransactionParams
        };

        return await HandleRequest(query, cancellationToken);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBankAccount([FromBody] CreateBankAccountRequest request, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateBankAccountCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }

    [HttpDelete(ApiRoutes.BankAccounts.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteBankAccount([FromQuery] string id,
                                                        CancellationToken cancellationToken = default)
    {
        var command = new DeleteBankAccountCommand 
        { 
            BankAccountId = Guid.Parse(id) 
        };

        return await HandleRequest(command, cancellationToken);
    }

    [HttpPut(ApiRoutes.BankAccounts.Activate)]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateBankAccount([FromQuery] string id,
                                                        CancellationToken cancellationToken = default)
    {
        var command = new ActivateBankAccountCommand 
        { 
            BankAccountId = Guid.Parse(id) 
        };

        return await HandleRequest(command, cancellationToken);
    }

    [HttpPut(ApiRoutes.BankAccounts.Deactivate)]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateBankAccount([FromQuery] string id, CancellationToken cancellationToken = default)
    {
        var command = new DeactivateBankAccountCommand 
        { 
            BankAccountId = Guid.Parse(id) 
        };

        return await HandleRequest(command, cancellationToken);
    }

    [HttpPost(ApiRoutes.BankAccounts.CashTransaction)]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateCashTransaction([FromRoute] string iban,
                                                           [FromBody] CreateCashTransactionRequest request,
                                                           CancellationToken cancellationToken = default)
    {
        if (iban != request.BaseCashTransaction.IBAN)
            return BadRequest("IBAN mismatch between route and request body.");


        return request.BaseCashTransaction.Type switch
        {
            CashTransactionType.Deposit =>
               await HandleRequest(_mapper.Map<MakeDepositCommand>(request), cancellationToken),

            CashTransactionType.Withdrawal =>
               await HandleRequest(_mapper.Map<MakeWithdrawalCommand>(request), cancellationToken),

            CashTransactionType.Transfer =>
               await HandleRequest(_mapper.Map<MakeFundsTransferCommand>(request), cancellationToken);
            
             _ => BadRequest("Unsupported transaction type.")
        };
    }

    [HttpPost(ApiRoutes.BankAccounts.FastTransaction)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateFastTransaction([FromRoute] string iban,
                                                           [FromBody] CreateFastTransactionRequest request,
                                                           CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateFastTransactionCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }

    [HttpPut(ApiRoutes.BankAccounts.IdRoute)]
    public async Task<IActionResult> AddOwnerToBankAccount([FromBody] AddOwnerToBankAccountRequest request,
                                                            CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<AddOwnerToBankAccountCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }
}