
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Constants;
using OnlineBanking.API.Extensions;
using OnlineBanking.API.Filters;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Features.CashTransactions.Queries;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.CashTransaction.Responses;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Helpers;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.API.Controllers;

[Authorize]
public class CashTransactionsController : BaseApiController
{
    // GET api/v1/cash-transactions/all?pageNumber=1&pageSize=50
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet(ApiRoutes.CashTransactions.All)]
    [ProducesResponseType(typeof(PagedList<CashTransactionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAllCashTransactions([FromQuery] CashTransactionParams cashTransactionParams,
                                                            CancellationToken cancellationToken = default)
    {
        var query = new GetAllCashTransactionsRequest() 
        { 
           CashTransactionParams = cashTransactionParams 
        };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        var cashTransactions = result.Payload.Data;

        if (cashTransactions.Any())
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                                         result.Payload.TotalCount, result.Payload.TotalPages);
        }

        return Ok(cashTransactions);
    }

    // GET api/v1/cash-transactions/12345678
    [HttpGet(ApiRoutes.CashTransactions.GetByAccountNo)]
    [ProducesResponseType(typeof(PagedList<CashTransactionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAccountCashTransactions([FromRoute] string accountNoOrIBAN,
                                                                 [FromQuery] CashTransactionParams cashTransactionParams,
                                                                 CancellationToken cancellationToken = default)
    {
        var query = new GetAccountTransactionsRequest() 
        { 
          AccountNoOrIBAN = accountNoOrIBAN,
          CashTransactionParams = cashTransactionParams
        };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        var accountTransactions = result.Payload.Data;

        if (accountTransactions.Any())
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                                         result.Payload.TotalCount, result.Payload.TotalPages);
        }

        return Ok(accountTransactions);
    }

    // GET api/v1/cash-transactions/TR12345678 
    [HttpGet(ApiRoutes.CashTransactions.GetByIBAN)]
    [ProducesResponseType(typeof(PagedList<CashTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCashTransactionsByIBAN([FromRoute] string iban,
                                                               [FromQuery] CashTransactionParams cashTransactionParams,
                                                               CancellationToken cancellationToken = default)
    {
        var query = new GetCashTransactionsByIBANRequest()
        {
            IBAN = iban,
            CashTransactionParams = cashTransactionParams
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        var accountTransactions = result.Payload.Data;

        if (accountTransactions.Any())
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                                         result.Payload.TotalCount, result.Payload.TotalPages);
        }

        return Ok(accountTransactions);
    }

    // POST api/v1/cash-transactions
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCashTransaction([FromRoute] string iban,
                                                           [FromBody] CreateCashTransactionRequest request,
                                                           CancellationToken cancellationToken = default)
    {
        var result = new ApiResult<Unit>();

        if (iban != request.BaseCashTransaction.IBAN)
            result.IsError = true;

        switch (request.BaseCashTransaction.Type)
        {
            case CashTransactionType.Deposit:
                var makeDepositCommand = _mapper.Map<MakeDepositCommand>(request);
                result = await _mediator.Send(makeDepositCommand, cancellationToken);
                break;

            case CashTransactionType.Withdrawal:
                var makeWithdrawalCommand = _mapper.Map<MakeWithdrawalCommand>(request);
                result = await _mediator.Send(makeWithdrawalCommand, cancellationToken);
                break;

            case CashTransactionType.Transfer or CashTransactionType.FAST:
                var makeFundsTransferCommand = _mapper.Map<MakeFundsTransferCommand>(request);
                result = await _mediator.Send(makeFundsTransferCommand, cancellationToken);
                break;

            default:
                break;
        }

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok();
    }

    // PUT api/v1/cash-transactions/1234
    [HttpPut(ApiRoutes.CashTransactions.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCashTransaction(UpdateCashTransactionRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateCashTransactionCommand() 
        { 
            Id = Guid.Parse(request.Id), 
            CashTransaction = request.CashTransaction
        };
        
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok();
    }

    // DELETE api/v1/cash-transactions/1234
    [HttpDelete(ApiRoutes.CashTransactions.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteCashTransaction([FromQuery] string id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteCashTransactionCommand() 
        { 
            Id = Guid.Parse(id) 
        };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok();
    }
}
