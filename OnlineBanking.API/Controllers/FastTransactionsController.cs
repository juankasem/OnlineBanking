using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Features.FastTransactions.Queries;
using OnlineBanking.Application.Models.FastTransaction.Requests;
using OnlineBanking.Application.Models.FastTransaction.Responses;

namespace OnlineBanking.API.Controllers;

[Authorize]
public class FastTransactionsController : BaseApiController
{
    // GET api/v1/FastTransactions/TR12345678 
    [HttpGet(ApiRoutes.FastTransactions.GetByIBAN)]
    [ProducesResponseType(typeof(IReadOnlyList<FastTransactionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListFastTransactionsByIBAN([FromRoute] string iban, CancellationToken cancellationToken = default)
    {
        var query = new GetFastTransactionsByIBANRequest()
        {
            IBAN = iban,
        };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    // POST api/v1/FastTransactions
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFastTransaction([FromBody] CreateFastTransactionRequest request, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateFastTransactionCommand>(request);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok();
    }

    // PUT api/v1/FastTransactions/1234
    [HttpPut(ApiRoutes.FastTransactions.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateFastTransaction([FromRoute] string id, [FromBody] UpdateFastTransactionRequest request,
                                                           CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateFastTransactionCommand>(request);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    // DELETE api/v1/FastTransactions/1234
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFastTransaction([FromRoute] string id, [FromBody] DeleteFastTransactionRequest request,
                                                            CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<DeleteFastTransactionCommand>(request);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }
}
