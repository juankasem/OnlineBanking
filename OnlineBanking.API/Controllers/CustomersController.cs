
namespace OnlineBanking.API.Controllers;

[Authorize]
public class CustomersController : BaseApiController
{
    [HttpGet(ApiRoutes.Customers.All)]
    [ProducesResponseType(typeof(PagedList<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAllCustomers([FromQuery] CustomerParams customerParams, CancellationToken cancellationToken = default)
    {
        var query = new GetAllCustomersRequest()
        {
            CustomerParams = customerParams
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        var customers = result.Payload.Data;

        if (customers.Any())
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                        result.Payload.TotalCount, result.Payload.TotalPages);
        }

        return Ok(customers);
    }

    [HttpGet(ApiRoutes.Customers.IdRoute)]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ValidateGuid("id")]
    public async Task<IActionResult> GetCustomerById([FromRoute] string id, CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerByIdRequest()
        {
            CustomerId = Guid.Parse(id)
        };

        return await HandleRequest(query, cancellationToken);
    }

    [HttpGet(ApiRoutes.Customers.BankAccounts)]
    [ProducesResponseType(typeof(List<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomerBankAccounts([FromRoute] string id,
                                                             CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerBankAccountsRequest() { CustomerId = Guid.Parse(id) };

        return await HandleRequest(query, cancellationToken);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request,
                                                    CancellationToken cancellationToken = default)
    {
        var query = _mapper.Map<CreateCustomerCommand>(request);

        return await HandleRequest(query, cancellationToken);
    }

    [HttpDelete(ApiRoutes.Customers.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCustomer([FromRoute] string customerId,
                                                    CancellationToken cancellationToken = default)
    {
        var query = new DeleteCustomerCommand()
        {
            CustomerId = Guid.Parse(customerId)
        };

        return await HandleRequest(query, cancellationToken);
    }
}