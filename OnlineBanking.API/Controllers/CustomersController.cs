using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Extensions;
using OnlineBanking.API.Helpers;
using OnlineBanking.Application.Features.Customers.Commands;
using OnlineBanking.Application.Features.Customers.Queries;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Application.Models.Customer.Requests;
using OnlineBanking.Application.Models.Customer.Responses;
using OnlineBanking.Core.Helpers;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.API.Controllers;

[Authorize]
public class CustomersController : BaseApiController
{
    [Cached(600)]
    [HttpGet(ApiRoutes.Customers.All)]
    [ProducesResponseType(typeof(PagedList<CustomerResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PagedList<CustomerResponse>>> ListAllCustomers([FromQuery] CustomerParams customerParams,
                                                                            CancellationToken cancellationToken = default)
    {
        var query = new GetAllCustomersRequest();
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) HandleErrorResponse(result.Errors);

        Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                                result.Payload.TotalCount, result.Payload.TotalPages);

        return Ok(result.Payload.Data);
    }
    
    [Cached(600)]
    [HttpGet(ApiRoutes.Customers.IdRoute)]
    [ProducesResponseType(typeof(CustomerListResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerResponse>> GetCustomerById([FromRoute] string customerId,
                                                                    [FromQuery] CustomerParams customerParams,
                                                                    CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerByIdRequest() { CustomerId = Guid.Parse(customerId) };
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    [Cached(600)]
    [HttpGet(ApiRoutes.Customers.BankAccounts)]
    [ProducesResponseType(typeof(List<BankAccountDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<BankAccountDto>>> GetCustomerBankAccounts([FromRoute] string customerId,
                                                                                [FromQuery] CustomerParams customerParams,
                                                                                CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerBankAccountsRequest() { CustomerId = Guid.Parse(customerId) };
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request,
                                                    CancellationToken cancellationToken = default)
    {
        var query = _mapper.Map<CreateCustomerCommand>(request);
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    [HttpDelete(ApiRoutes.Customers.IdRoute)]
    public async Task<IActionResult> DeleteCustomer([FromRoute] string customerId,
                                                    CancellationToken cancellationToken = default)
    {
        var query = new DeleteCustomerCommand() { CustomerId = Guid.Parse(customerId) };
        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }
}