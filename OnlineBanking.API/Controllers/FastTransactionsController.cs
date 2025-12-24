
using OnlineBanking.Application.Features.FastTransactions.Create;
using OnlineBanking.Application.Features.FastTransactions.Delete;
using OnlineBanking.Application.Features.FastTransactions.GetByIBAN;
using OnlineBanking.Application.Features.FastTransactions.Update;

namespace OnlineBanking.API.Controllers;

/// <summary>
/// API controller for fast transaction management.
/// Provides endpoints for retrieving, creating, updating, and deleting fast transactions.
/// Fast transactions enable quick fund transfers between bank accounts.
/// All operations require authorization and bank account ownership validation.
/// </summary>
[Authorize]
public class FastTransactionsController : BaseApiController
{    
    #region GET Operations

    /// <summary>
    /// Retrieves all fast transactions for a specific IBAN.
    /// </summary>
    /// <param name="iban">IBAN or account number to filter transactions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of fast transactions for the specified account</returns>
    [HttpGet(ApiRoutes.FastTransactions.GetByIBAN)]
    [ProducesResponseType(typeof(PagedList<FastTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListFastTransactionsByIBAN([FromRoute] string iban,
                                                                [FromQuery] FastTransactionParams fastTransactionsParams,
                                                                CancellationToken cancellationToken = default)
    {
        var query = new GetFastTransactionsByIBANRequest()
        {
            IBAN = iban,
            FastTransactionParams = fastTransactionsParams
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) 
            return HandleErrorResponse(result.Errors);

        var fastTransactions = result.Payload?.Data ?? [];

        if (fastTransactions.Any())
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage,
                                         result.Payload.PageSize,
                                         result.Payload.TotalCount, 
                                         result.Payload.TotalPages);
        }

        return Ok(fastTransactions);
    }

    #endregion

    #region POST Operations

    /// <summary>
    /// Creates a new fast transaction.
    /// Requires bank account ownership validation.
    /// </summary>
    /// <param name="request">Fast transaction creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created fast transaction details</returns>
    [HttpPost]
    [ValidateBankAccountOwner("iban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateFastTransaction([FromBody] CreateFastTransactionRequest request, 
                                                            CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateFastTransactionCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }

    #endregion

    #region PUT Operations

    /// <summary>
    /// Updates an existing fast transaction.
    /// </summary>
    /// <param name="id">Fast transaction ID to update</param>
    /// <param name="request">Fast transaction update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated fast transaction details</returns>
    [HttpPut(ApiRoutes.FastTransactions.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ValidateGuid("id")]
    public async Task<IActionResult> UpdateFastTransaction([FromRoute] Guid id, 
                                                           [FromBody] UpdateFastTransactionRequest request,
                                                            CancellationToken cancellationToken = default)
    {
        if (id != request.Id)
            return HandleErrorResponse([new Error(ErrorCode.BadRequest, 
                                       "Id mismatch between route and request body.")]);

        var command = _mapper.Map<UpdateFastTransactionCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }

    #endregion

    #region DELETE Operations

    /// <summary>
    /// Deletes a fast transaction.
    /// </summary>
    /// <param name="iban">IBAN of the account</param>
    /// <param name="id">Fast transaction ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on successful deletion</returns>    
    [HttpDelete(ApiRoutes.FastTransactions.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteFastTransaction([FromRoute] string iban,
                                                           [FromRoute] Guid id,
                                                           CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return BadRequest("IBAN is required");

        var command = new DeleteFastTransactionCommand()
        {
            Id = id,
            IBAN = iban
        };

        return await HandleRequest(command, cancellationToken);
    }

    #endregion
}
