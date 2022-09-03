using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.Application.Features.BankAccounts.Commands;
using OnlineBanking.Application.Features.BankAccounts.Queries;
using OnlineBanking.Application.Models.BankAccount.Requests;
using OnlineBanking.Application.Models.BankAccount.Responses;

namespace OnlineBanking.API.Controllers;

[Authorize]
public class BankAccountsController : BaseApiController
{
    [HttpGet(ApiRoutes.BankAccounts.All)]
    [ProducesResponseType(typeof(BankAccountListResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<BankAccountListResponse>> ListAllBankAccounts(CancellationToken cancellationToken = default)
    {
        var query = new GetAllBankAccountsRequest();
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    [HttpGet(ApiRoutes.BankAccounts.GetByCustomerNo)]
    [ProducesResponseType(typeof(BankAccountListResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<BankAccountListResponse>> GetCustomerBankAccounts([FromRoute] string customerNo,
                                                                                    CancellationToken cancellationToken = default)
    {
        var query = new GetBankAccountsByCustomerNoRequest() { CustomerNo = customerNo };
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    [HttpGet(ApiRoutes.BankAccounts.GetByAccountNo)]
    [ProducesResponseType(typeof(BankAccountResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<BankAccountResponse>> GetBankAccountByAccountNo(string accountNo,
                                                                                    CancellationToken cancellationToken = default)
    {
        var query = new GetBankAccountByAccountNoRequest() { AccountNo = accountNo };
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    [HttpGet(ApiRoutes.BankAccounts.AccountTransactions)]
    [ProducesResponseType(typeof(BankAccountResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<BankAccountResponse>> GetAccountByIBANWithTransactions(string iban, CancellationToken cancellationToken = default)
    {
        var query = new GetBankAccountWithTransactionsRequest() { IBAN = iban };
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBankAccount([FromBody] CreateBankAccountRequest request,
                                                        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateBankAccountCommand>(request);
        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    [HttpDelete(ApiRoutes.BankAccounts.IdRoute)]
    public async Task<IActionResult> DeleteBankAccount([FromQuery] string id,
                                                        CancellationToken cancellationToken = default)
    {
        var command = new DeleteBankAccountCommand { BankAccountId = Guid.Parse(id) };
        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    [HttpPut(ApiRoutes.BankAccounts.Activate)]
    public async Task<IActionResult> ActivateBankAccount([FromQuery] string id,
                                                        CancellationToken cancellationToken = default)
    {
        var command = new ActivateBankAccountCommand { BankAccountId = Guid.Parse(id) };
        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    [HttpPut(ApiRoutes.BankAccounts.Deactivate)]
    public async Task<IActionResult> DeactivateBankAccount([FromQuery] string id,
                                                            CancellationToken cancellationToken = default)
    {
        var command = new DeactivateBankAccountCommand { BankAccountId = Guid.Parse(id) };

        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    [HttpPut(ApiRoutes.BankAccounts.IdRoute)]
    public async Task<IActionResult> AddOwnerToBankAccount([FromBody] AddOwnerToBankAccountRequest request,
                                                            CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<AddOwnerToBankAccountCommand>(request);

        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }
}