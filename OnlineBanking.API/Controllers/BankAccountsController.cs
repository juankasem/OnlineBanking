using OnlineBanking.Application.Features.BankAccounts.Activate;
using OnlineBanking.Application.Features.BankAccounts.AddOwner;
using OnlineBanking.Application.Features.BankAccounts.Create;
using OnlineBanking.Application.Features.BankAccounts.Deactivate;
using OnlineBanking.Application.Features.BankAccounts.Delete;
using OnlineBanking.Application.Features.BankAccounts.GetAll;
using OnlineBanking.Application.Features.BankAccounts.GetByAccountNo;
using OnlineBanking.Application.Features.BankAccounts.GetByCustomerNo;
using OnlineBanking.Application.Features.BankAccounts.GetWithTransactions;
using OnlineBanking.Application.Features.CashTransactions.Create.Deposit;
using OnlineBanking.Application.Features.CashTransactions.Create.Transfer;
using OnlineBanking.Application.Features.CashTransactions.Create.Withdraw;
using OnlineBanking.Application.Features.FastTransactions.Create;
using OnlineBanking.Application.Features.FastTransactions.Delete;

namespace OnlineBanking.API.Controllers;

/// <summary>
/// API controller for bank account management.
/// Handles account retrieval, creation, deletion, activation, and transaction processing.
/// Supports cash transactions (Deposit, Withdrawal, Transfer) and fast transactions.
/// All operations require authorization; creation operations require Admin role.
/// </summary>
[Authorize]
public class BankAccountsController : BaseApiController
{
    #region GET Operations

    /// <summary>
    /// Retrieves all bank accounts with pagination.
    /// </summary>
    /// <param name="bankAccountParams">Pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of bank accounts</returns>
    [HttpGet(ApiRoutes.BankAccounts.All)]
    [ProducesResponseType(typeof(PagedList<BankAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAllBankAccounts(
        [FromQuery] BankAccountParams bankAccountParams, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllBankAccountsRequest()
        {
            BankAccountParams = bankAccountParams
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) 
            return HandleErrorResponse(result.Errors);

        var bankAccounts = result.Payload?.Data ?? [];

        if (bankAccounts.Any())
        {
            Response.AddPaginationHeader(
                result.Payload.CurrentPage, 
                result.Payload.PageSize,
                result.Payload.TotalCount, 
                result.Payload.TotalPages);
        }

        return Ok(bankAccounts);
    }

    /// <summary>
    /// Retrieves bank accounts by customer number with pagination.
    /// </summary>
    /// <param name="customerNo">Customer number to filter by</param>
    /// <param name="bankAccountParams">Bank account pagination parameters</param>
    /// <param name="accountTransactionsParams">Transaction pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of bank accounts for the customer</returns>
    [HttpGet(ApiRoutes.BankAccounts.GetByCustomerNo)]
    [ProducesResponseType(typeof(PagedList<BankAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBankAccountsByCustomerNo(
        [FromRoute] string customerNo,
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

        if (result.IsError) 
            return HandleErrorResponse(result.Errors);

        var bankAccounts = result.Payload?.Data ?? [];

        if (bankAccounts.Any())
        {
            Response.AddPaginationHeader(
                result.Payload.CurrentPage, 
                result.Payload.PageSize,
                result.Payload.TotalCount, 
                result.Payload.TotalPages);
        }

        return Ok(bankAccounts);
    }

    /// <summary>
    /// Retrieves a bank account by account number.
    /// </summary>
    /// <param name="accountNo">Account number</param>
    /// <param name="accountTransactionsParams">Transaction pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bank account details with transactions</returns>
    [HttpGet(ApiRoutes.BankAccounts.GetByAccountNo)]
    [ValidateBankAccountOwner("account-no")]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBankAccountByAccountNo(
        [FromRoute(Name = "account-no")] string accountNo,
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

    /// <summary>
    /// Retrieves a bank account by IBAN.
    /// </summary>
    /// <param name="iban">International Bank Account Number</param>
    /// <param name="accountTransactionParams">Transaction pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bank account details with transactions</returns>
    [HttpGet(ApiRoutes.BankAccounts.GetByIBAN)]
    [ValidateBankAccountOwner("iban")]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBankAccountByIBAN(
        [FromRoute] string iban,
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
    #endregion

    #region POST Operations

    /// <summary>
    /// Creates a new bank account.
    /// Requires Admin role authorization.
    /// </summary>
    /// <param name="request">Bank account creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created bank account details</returns>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateBankAccount(
        [FromBody] CreateBankAccountRequest request, 
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateBankAccountCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }

    // <summary>
    /// Creates a cash transaction (Deposit, Withdrawal, or Transfer).
    /// Routes the request to the appropriate handler based on transaction type.
    /// </summary>
    /// <param name="iban">IBAN of the account initiating the transaction</param>
    /// <param name="request">Cash transaction creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction creation result</returns>
    [HttpPost(ApiRoutes.BankAccounts.CashTransaction)]
    [ValidateBankAccountOwner("iban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateCashTransaction(
        [FromRoute] string iban,
        [FromBody] CreateCashTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (iban != request.BaseCashTransaction.IBAN)
            return HandleErrorResponse([new Error(ErrorCode.BadRequest,
                CashTransactionErrorMessages.IBANMismatch)]
           );

        return request.BaseCashTransaction.Type switch
        {
            CashTransactionType.Deposit =>
               await HandleRequest(_mapper.Map<MakeDepositCommand>(request), cancellationToken),

            CashTransactionType.Withdrawal =>
               await HandleRequest(_mapper.Map<MakeWithdrawalCommand>(request), cancellationToken),

            CashTransactionType.Transfer =>
               await HandleRequest(_mapper.Map<MakeFundsTransferCommand>(request), cancellationToken),

            _ => HandleErrorResponse([new Error(ErrorCode.BadRequest,
                string.Format(CashTransactionErrorMessages.UnsupportedTransactionType, 
                request.BaseCashTransaction.Type))])
        };
    }

    /// <summary>
    /// Creates a fast transaction for quick fund transfers.
    /// </summary>
    /// <param name="iban">IBAN of the account initiating the transaction</param>
    /// <param name="request">Fast transaction creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction creation result</returns>
    [HttpPost(ApiRoutes.BankAccounts.FastTransaction)]
    [ValidateBankAccountOwner("iban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateFastTransaction(
        [FromRoute] string iban,
        [FromBody] CreateFastTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateFastTransactionCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }

    #endregion

    #region PUT Operations

    /// <summary>
    /// Activates a bank account.
    /// </summary>
    /// <param name="bankAccountId">Bank account ID to activate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated bank account details</returns>
    [HttpPut(ApiRoutes.BankAccounts.Activate)]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateBankAccount(
        [FromQuery(Name = "id")] string bankAccountId,
        CancellationToken cancellationToken = default)
    {
        var command = new ActivateBankAccountCommand
        {
            BankAccountId = Guid.Parse(bankAccountId)
        };

        return await HandleRequest(command, cancellationToken);
    }

    /// <summary>
    /// Deactivates a bank account.
    /// </summary>
    /// <param name="id">Bank account ID to deactivate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated bank account details</returns>
    [HttpPut(ApiRoutes.BankAccounts.Deactivate)]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateBankAccount(
        [FromQuery(Name = "id")] string bankAccountId, 
        CancellationToken cancellationToken = default)
    {
        var command = new DeactivateBankAccountCommand()
        {
            BankAccountId = Guid.Parse(bankAccountId)
        };

        return await HandleRequest(command, cancellationToken);
    }

    /// <summary>
    /// Adds an owner to a bank account.
    /// </summary>
    /// <param name="request">Add owner request with account and customer IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated bank account with new owner</returns>
    [HttpPut(ApiRoutes.BankAccounts.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddOwnerToBankAccount(
        [FromBody] AddOwnerToBankAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<AddOwnerToBankAccountCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }
    #endregion

    #region DELETE Operations

    /// <summary>
    /// Deletes a bank account.
    /// </summary>
    /// <param name="bankAccountId">Bank account ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on successful deletion</returns>
    [HttpDelete(ApiRoutes.BankAccounts.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ValidateGuid("bankAccountId")]
    public async Task<IActionResult> DeleteBankAccount(
        [FromQuery(Name ="id")] string bankAccountId,
         CancellationToken cancellationToken = default)
    {
        var command = new DeleteBankAccountCommand()
        {
            BankAccountId = Guid.Parse(bankAccountId)
        };

        return await HandleRequest(command, cancellationToken);
    }

    /// <summary>
    /// Deletes a fast transaction.
    /// </summary>
    /// <param name="iban">IBAN of the account</param>
    /// <param name="id">Fast transaction ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on successful deletion</returns>  
    [HttpDelete(ApiRoutes.BankAccounts.FastTransactionById)]
    [ValidateBankAccountOwner("iban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFastTransaction(
        [FromRoute] string iban,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return BadRequest("IBAN is required");

        if (id == Guid.Empty)
            return BadRequest("Transaction ID cannot be empty");

        var command = new DeleteFastTransactionCommand()
        {
            Id = id,
            IBAN = iban
        };

        return await HandleRequest(command, cancellationToken);
    }

    #endregion
}