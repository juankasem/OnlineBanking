using System.Collections.Immutable;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Filters;
using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Features.FastTransactions.Queries;
using OnlineBanking.Application.Models.FastTransaction.Requests;
using OnlineBanking.Application.Models.FastTransaction.Responses;

namespace OnlineBanking.API.Controllers;

public class FastTransactionsController : BaseApiController
{
    // GET api/v1/Fast-transactions/TR12345678 
    [HttpGet(ApiRoutes.FastTransactions.GetByIBAN)]
    [ProducesResponseType(typeof(ImmutableList<FastTransactionResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ImmutableList<FastTransactionResponse>>> GetFastTransactionsByIBAN([FromRoute] string iban,
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
    [HttpDelete(ApiRoutes.FastTransactions.IdRoute)]
    [ValidateGuid]
    public async Task<IActionResult> DeleteFastTransaction([FromQuery] string id,
                                                            CancellationToken cancellationToken = default)
    {
        var command = new DeleteFastTransactionCommand() { Id = Guid.Parse(id) };
        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }
}
