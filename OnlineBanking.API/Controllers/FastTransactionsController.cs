
using OnlineBanking.Application.Features.FastTransactions.Create;
using OnlineBanking.Application.Features.FastTransactions.Delete;
using OnlineBanking.Application.Features.FastTransactions.GetByIBAN;
using OnlineBanking.Application.Features.FastTransactions.Update;

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
    [ValidateBankAccountOwner("iban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFastTransaction([FromBody] CreateFastTransactionRequest request, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateFastTransactionCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }

    // PUT api/v1/FastTransactions/1234
    [HttpPut(ApiRoutes.FastTransactions.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateFastTransaction([FromRoute] string id, [FromBody] UpdateFastTransactionRequest request,
                                                           CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateFastTransactionCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }

    // DELETE api/v1/FastTransactions/1234
    [HttpDelete(ApiRoutes.FastTransactions.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFastTransaction([FromRoute] string iban,
                                                           [FromRoute] Guid id,
                                                           CancellationToken cancellationToken = default)
    {
        var command = new DeleteFastTransactionCommand()
        {
            Id = id,
            IBAN = iban
        };

        return await HandleRequest(command, cancellationToken);
    }
}
