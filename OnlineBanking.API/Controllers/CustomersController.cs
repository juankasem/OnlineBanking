using OnlineBanking.Application.Features.Customers.Create;
using OnlineBanking.Application.Features.Customers.Delete;
using OnlineBanking.Application.Features.Customers.GetAll;
using OnlineBanking.Application.Features.Customers.GetBankAccounts;
using OnlineBanking.Application.Features.Customers.GetById;

namespace OnlineBanking.API.Controllers;

/// <summary>
/// API controller for customer management.
/// Provides endpoints for retrieving, creating, and deleting customers.
/// Also supports retrieving customer-associated bank accounts.
/// All operations require authorization.
/// </summary>
[Authorize]
public class CustomersController : BaseApiController
{
    #region GET Operations

    [HttpGet(ApiRoutes.Customers.All)]
    [ProducesResponseType(typeof(PagedList<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAllCustomers([FromQuery] CustomerParams customerParams, 
        CancellationToken cancellationToken = default)
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
            Response.AddPaginationHeader(result.Payload.CurrentPage, 
                result.Payload.PageSize,
                result.Payload.TotalCount, 
                result.Payload.TotalPages);
        }

        return Ok(customers);
    }

    /// <summary>
    /// Retrieves a specific customer by ID.
    /// </summary>
    /// <param name="customerId">Customer ID (must be a valid GUID)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer details</returns>
    [HttpGet(ApiRoutes.Customers.IdRoute)]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ValidateGuid("id")]
    public async Task<IActionResult> GetCustomerById([FromRoute(Name ="id")] string customerId, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerByIdRequest()
        {
            CustomerId = Guid.Parse(customerId)
        };

        return await HandleRequest(query, cancellationToken);
    }

    /// <summary>
    /// Retrieves all bank accounts associated with a specific customer.
    /// </summary>
    /// <param name="customerNo">Customer No.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of customer's bank accounts</returns>
    [HttpGet(ApiRoutes.Customers.BankAccounts)]
    [ProducesResponseType(typeof(List<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomerBankAccounts([FromRoute] string customerNo,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerBankAccountsRequest() 
        { 
            CustomerNo = customerNo
        };

        return await HandleRequest(query, cancellationToken);
    }

    #endregion

    #region POST Operations

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="request">Customer creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created customer details</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _mapper.Map<CreateCustomerCommand>(request);

        return await HandleRequest(query, cancellationToken);
    }

    #endregion

    #region DELETE Operations

    /// <summary>
    /// Deletes a customer.
    /// </summary>
    /// <param name="customerId">Customer ID to delete (must be a valid GUID)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on successful deletion</returns>
    [HttpDelete(ApiRoutes.Customers.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteCustomer([FromRoute] string customerId,
        CancellationToken cancellationToken = default)
    {
        var query = new DeleteCustomerCommand()
        {
            CustomerId = Guid.Parse(customerId)
        };

        return await HandleRequest(query, cancellationToken);
    }

    #endregion
}