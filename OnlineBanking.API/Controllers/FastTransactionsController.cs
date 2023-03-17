using System.Net;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Filters;
using OnlineBanking.API.Helpers;
using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Features.FastTransactions.Queries;
using OnlineBanking.Application.Models.FastTransaction.Requests;
using OnlineBanking.Application.Models.FastTransaction.Responses;

namespace OnlineBanking.API.Controllers;

public class FastTransactionsController : BaseApiController
{
    // GET api/v1/fast-transactions/TR12345678 
    [Cached(600)]
    [HttpGet(ApiRoutes.FastTransactions.GetByIBAN)]
    [ProducesResponseType(typeof(IReadOnlyList<FastTransactionResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<FastTransactionResponse>>> ListFastTransactionsByIBAN([FromRoute] string iban,
                                                                                                CancellationToken cancellationToken = default)
    {
        var query = new GetFastTransactionsByIBANRequest()
        {
            IBAN = iban,
        };
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    // POST api/v1/Fast-transactions
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateFastTransaction([FromBody] CreateFastTransactionRequest request,
                                                        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateFastTransactionCommand>(request);

        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    // PUT api/v1/Fast-transactions/1234
    [HttpPut(ApiRoutes.FastTransactions.IdRoute)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ValidateGuid]
    public async Task<IActionResult> UpdateFastTransaction(Guid id, [FromBody] UpdateFastTransactionRequest request,
                                                        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateFastTransactionCommand>(request);

        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    // DELETE api/v1/Fast-transactions/1234
    [HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ValidateGuid]
    public async Task<IActionResult> DeleteFastTransaction([FromBody] DeleteFastTransactionRequest request,
                                                            CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<DeleteFastTransactionCommand>(request);

        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }
}
