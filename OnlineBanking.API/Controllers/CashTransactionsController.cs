
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Common;
using OnlineBanking.API.Constants;
using OnlineBanking.API.Extensions;
using OnlineBanking.API.Filters;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Features.CashTransactions.Queries;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.CashTransaction.Responses;
using OnlineBanking.Core.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


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

        if (cashTransactions.Count > 0)
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                                         result.Payload.TotalCount, result.Payload.TotalPages);
        }

        return Ok(cashTransactions);
    }

    // GET api/v1/cash-transactions/TR12345678 
    [HttpGet(ApiRoutes.CashTransactions.GetByIBAN)]
    [ProducesResponseType(typeof(PagedList<CashTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCashTransactionsByAccountNoOrIBAN([FromRoute] string iban,
                                                               [FromQuery] CashTransactionParams cashTransactionParams,
                                                               CancellationToken cancellationToken = default)
    {
        var query = new GetCashTransactionsByAccountNoOrIBANRequest()
        {
            IBAN = iban,
            CashTransactionParams = cashTransactionParams
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        var accountTransactions = result.Payload.Data;

        if (accountTransactions.Count > 0)
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                                         result.Payload.TotalCount, result.Payload.TotalPages);
        }

        return Ok(accountTransactions);
    }

    // POST api/v1/cash-transactions
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
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
               await HandleRequest(_mapper.Map<MakeFundsTransferCommand>(request), cancellationToken),
            
             _ => BadRequest("Unsupported transaction type.")
        };
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

        return await HandleRequest(command, cancellationToken);
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
        
        return await HandleRequest(command, cancellationToken);
    }
}
