using OnlineBanking.Application.Features.CashTransactions.Create.Deposit;
using OnlineBanking.Application.Features.CashTransactions.Create.Transfer;
using OnlineBanking.Application.Features.CashTransactions.Create.Withdraw;
using OnlineBanking.Application.Features.CashTransactions.Delete;
using OnlineBanking.Application.Features.CashTransactions.GetAll;
using OnlineBanking.Application.Features.CashTransactions.GetByAccountNoOrIBAN;
using OnlineBanking.Application.Features.CashTransactions.Update;

namespace OnlineBanking.API.Controllers;

/// <summary>
/// API controller for cash transaction management.
/// Provides endpoints for retrieving, creating, updating, and deleting cash transactions.
/// Supports transaction types: Deposit, Withdrawal, and Transfer.
/// All operations require authorization; list operations require Admin role.
/// </summary>
[Authorize]
public class CashTransactionsController : BaseApiController
{
    #region GET Operations

    /// <summary>
    /// Retrieves all cash transactions with pagination.
    /// Restricted to Admin users only.
    /// </summary>
    /// <param name="cashTransactionParams">Pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of all cash transactions</returns>
    // GET api/v1/cash-transactions/all?pageNumber=1&pageSize=50
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet(ApiRoutes.CashTransactions.All)]
    [ProducesResponseType(typeof(PagedList<CashTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListAllCashTransactions(
        [FromQuery] CashTransactionParams cashTransactionParams,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCashTransactionsRequest()
        {
            CashTransactionParams = cashTransactionParams
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) 
            return HandleErrorResponse(result.Errors);

        var cashTransactions = result.Payload?.Data ?? [];

        if (cashTransactions.Count > 0)
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, 
                result.Payload.PageSize,
                result.Payload.TotalCount, 
                result.Payload.TotalPages);
        }

        return Ok(cashTransactions);
    }

    // GET api/v1/cash-transactions/TR12345678 
    [HttpGet(ApiRoutes.CashTransactions.AccountNoOrIBAN)]
    [ProducesResponseType(typeof(PagedList<CashTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCashTransactionsByAccountNoOrIBAN(
        [FromRoute(Name = "iban")] string accountNoOrIBAN,
        [FromQuery] CashTransactionParams cashTransactionParams,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCashTransactionsByAccountNoOrIBANRequest()
        {
            AccountNoOrIBAN = accountNoOrIBAN,
            CashTransactionParams = cashTransactionParams
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) 
            return HandleErrorResponse(result.Errors);

        var cashTransactions = result.Payload?.Data ?? [];

        if (cashTransactions.Count > 0)
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, 
                result.Payload.PageSize,
                result.Payload.TotalCount, 
                result.Payload.TotalPages);
        }

        return Ok(cashTransactions);
    }

    #endregion

    #region POST Operations

    /// <summary>
    /// Creates a new cash transaction (Deposit, Withdrawal, or Transfer).
    /// Routes the request to the appropriate handler based on transaction type.
    /// </summary>
    /// <param name="iban">IBAN of the account initiating the transaction</param>
    /// <param name="request">Cash transaction creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction creation result</returns>
    [HttpPost(ApiRoutes.CashTransactions.AccountNoOrIBAN)]
    [ValidateBankAccountOwner("iban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateCashTransaction(
        [FromRoute] string iban,
        [FromBody] CreateCashTransactionRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return BadRequest("IBAN is required");

        if (!iban.Equals(request.BaseCashTransaction.IBAN, StringComparison.OrdinalIgnoreCase))
        {
            var error = new Error(ErrorCode.BadRequest, "IBAN mismatch between route and request body");
            return HandleErrorResponse([error]);
        }

        return request.BaseCashTransaction.Type switch
        {
            CashTransactionType.Deposit =>
               await HandleRequest(_mapper.Map<MakeDepositCommand>(request), cancellationToken),

            CashTransactionType.Withdrawal =>
               await HandleRequest(_mapper.Map<MakeWithdrawalCommand>(request), cancellationToken),

            CashTransactionType.Transfer =>
               await HandleRequest(_mapper.Map<MakeFundsTransferCommand>(request), cancellationToken),

            _ => HandleErrorResponse([new Error(ErrorCode.BadRequest,
                $"Unsupported transaction type: {request.BaseCashTransaction.Type}")])
        };
    }

    #endregion

    #region PUT Operations

    /// <summary>
    /// Updates an existing cash transaction.
    /// </summary>
    /// <param name="request">Cash transaction update request containing transaction ID</param>
    /// <param name="cancellationToken">Cancellation token</param>    
    [HttpPut(ApiRoutes.CashTransactions.AccountNoOrIBAN)]
    [ValidateBankAccountOwner("iban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCashTransaction(
        UpdateCashTransactionRequest request, 
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateCashTransactionCommand()
        {
            Id = Guid.Parse(request.Id),
            CashTransaction = request.CashTransaction
        };

        return await HandleRequest(command, cancellationToken);
    }

    #endregion

    #region DELETE Operations

    /// <summary>
    /// Deletes a cash transaction by ID.
    /// </summary>
    /// <param name="id">Transaction ID to delete (must be a valid GUID)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on successful deletion</returns>
    [HttpDelete(ApiRoutes.CashTransactions.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteCashTransaction(
        [FromQuery] string id, 
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteCashTransactionCommand()
        {
            Id = Guid.Parse(id)
        };

        return await HandleRequest(command, cancellationToken);
    }

    #endregion
}
