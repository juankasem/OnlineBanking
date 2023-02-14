using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Filters;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Features.CashTransactions.Queries;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.CashTransaction.Responses;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.API.Controllers;

[Authorize]
public class CashTransactionsController : BaseApiController
{
    // GET api/v1/cash-transactions/all?pageNumber=1&pageSize=50
    [Authorize(Roles = "Admin")]
    [HttpGet(ApiRoutes.CashTransactions.All)]
    [ProducesResponseType(typeof(CashTransactionListResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CashTransactionListResponse>> ListAllCashTransactions([FromQuery] CashTransactionParams cashTransactionParams,
                                                                                         CancellationToken cancellationToken = default)
    {
        var query = new GetAllCashTransactionsRequest();
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    // GET api/v1/cash-transactions/12345678
    [HttpGet(ApiRoutes.CashTransactions.GetByAccountNo)]
    [ProducesResponseType(typeof(CashTransactionListResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CashTransactionListResponse>> GetCashTransactionsByAccountNo([FromRoute] string accountNo,
                                                                                                [FromQuery] CashTransactionParams cashTransactionParams,
                                                                                                CancellationToken cancellationToken = default)
    {
        var query = new GetCashTransactionsByAccountNoRequest() { AccountNo = accountNo };
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    // GET api/v1/cash-transactions/TR12345678 
    [HttpGet(ApiRoutes.CashTransactions.GetByIBAN)]
    [ProducesResponseType(typeof(CashTransactionListResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CashTransactionListResponse>> GetCashTransactionsByIBAN([FromRoute] string iban,
                                                                                           [FromQuery] CashTransactionParams cashTransactionParams,
                                                                                           CancellationToken cancellationToken = default)
    {
        var query = new GetCashTransactionsByIBANRequest() { IBAN = iban };
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    // POST api/v1/cash-transactions
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostCashTransaction([FromBody] CreateCashTransactionRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        var result = new ApiResult<Unit>();

        switch (request.BaseCashTransaction.Type)
        {
            case CashTransactionType.Deposit:
                var makeDepositCommand = _mapper.Map<MakeDepositCommand>(request);
                result = await _mediator.Send(makeDepositCommand);
                break;

            case CashTransactionType.Withdrawal:
                var makeWithdrawalCommand = _mapper.Map<MakeWithdrawalCommand>(request);
                result = await _mediator.Send(makeWithdrawalCommand);
                break;

            case CashTransactionType.Transfer or CashTransactionType.FAST:
                var makeFundsTransferCommand = _mapper.Map<MakeFundsTransferCommand>(request);
                result = await _mediator.Send(makeFundsTransferCommand);
                break;

            default:
                break;
        }

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    // PUT api/v1/cash-transactions/1234
    [HttpPut(ApiRoutes.CashTransactions.IdRoute)]
    public async Task<IActionResult> UpdateCashTransaction(UpdateCashTransactionRequest request,
                                                            CancellationToken cancellationToken = default)
    {
        var command = new UpdateCashTransactionCommand() 
        { 
            Id = Guid.Parse(request.Id), 
            BaseCasTransaction = request.BaseCashTransaction
        };
        
        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    // DELETE api/v1/cash-transactions/1234
    [HttpDelete(ApiRoutes.CashTransactions.IdRoute)]
    [ValidateGuid]
    public async Task<IActionResult> DeleteCashTransaction([FromQuery] string id,
                                                            CancellationToken cancellationToken = default)
    {
        var command = new DeleteCashTransactionCommand() { Id = Guid.Parse(id) };
        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }
}
