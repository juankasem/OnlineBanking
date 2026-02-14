using OnlineBanking.Application.Extensions;

namespace OnlineBanking.Application.Features.BankAccounts.GetByCustomerNo;

/// <summary>
/// Handles requests to retrieve paginated bank accounts for a specific customer.
/// Includes associated account owners and cash transactions for each account.
/// </summary>
public class GetBankAccountsByCustomerNoRequestHandler(
    IUnitOfWork uow,
    IBankAccountMapper bankAccountMapper,
    ILogger<GetBankAccountsByCustomerNoRequestHandler> logger) : 
    IRequestHandler<GetBankAccountsByCustomerNoRequest, ApiResult<PagedList<BankAccountResponse>>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountMapper _bankAccountMapper = bankAccountMapper;
    private readonly ILogger<GetBankAccountsByCustomerNoRequestHandler> _logger = logger;

    /// <summary>
    /// Handles the request to retrieve bank accounts for a customer.
    /// </summary>
    public async Task<ApiResult<PagedList<BankAccountResponse>>> Handle(
        GetBankAccountsByCustomerNoRequest request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = new ApiResult<PagedList<BankAccountResponse>>();
        var customerNo = request.CustomerNo;

        _logger.LogInformation(
        "Retrieving bank accounts for customer: {CustomerNo}",
        customerNo);

        if (!await _uow.Customers.ExistsAsync(customerNo))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(CustomerErrorMessages.NotFound, "No.", customerNo));

            return result;
        }

        var bankAccountParams = request.BankAccountParams;
        var accountTransactionsParams = request.AccountTransactionsParams;
        var customerBankAccounts = new List<BankAccountResponse>();

        var (bankAccounts, totalCount) = await _uow.BankAccounts.GetBankAccountsByCustomerNoAsync(request.CustomerNo, bankAccountParams);

        if (bankAccounts.Count == 0)
        {
            _logger.LogInformation("No bank accounts found for customer: {CustomerNo}", 
                customerNo);

            result.Payload = PagedList<BankAccountResponse>.Create([], 0, 0, 0);
            return result;
        }

        foreach (var bankAccount in bankAccounts)
        {
            var bankAccountOwners = await _uow.Customers.GetByIBANAsync(bankAccount.IBAN);
            var (cashTransactions, transactionsCount) = await _uow.CashTransactions.GetByIBANAsync(bankAccount.IBAN, accountTransactionsParams);

            var cashTransactionsPagedList = cashTransactions.ToPagedList(
                transactionsCount,
                accountTransactionsParams.PageNumber,
                accountTransactionsParams.PageSize,
                cancellationToken);

            var customerBankAccount = _bankAccountMapper.MapToResponseModel(
                bankAccount, 
                bankAccountOwners, 
                cashTransactionsPagedList);

            customerBankAccounts.Add(customerBankAccount);
        }

        result.Payload = customerBankAccounts
            .AsReadOnly()
            .ToPagedList(
            customerBankAccounts.Count,
            bankAccountParams.PageNumber,
            bankAccountParams.PageSize,
            cancellationToken);

        return result;
    }
}