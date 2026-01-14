using OnlineBanking.Application.Features.Branch.Create;
using OnlineBanking.Application.Features.Branch.Delete;
using OnlineBanking.Application.Features.Branch.GetAll;
using OnlineBanking.Application.Features.Branch.GetById;
using OnlineBanking.Application.Features.Branch.Update;

namespace OnlineBanking.API.Controllers;

/// <summary>
/// API controller for branch management.
/// Provides endpoints for retrieving, creating, updating, and deleting bank branches.
/// All operations require authorization; create operations require Admin role.
/// </summary>
[Authorize]
public class BranchesController : BaseApiController
{
    #region GET Operations

    /// <summary>
    /// Retrieves all branches with pagination.
    /// </summary>
    /// <param name="branchParams">Pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of branches</returns>
    [HttpGet(ApiRoutes.Branches.All)]
    [ProducesResponseType(typeof(PagedList<BranchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAllBranches([FromQuery] BranchParams branchParams, 
        CancellationToken cancellationToken = default)
    {
        var request = new GetAllBranchesRequest()
        {
            BranchParams = branchParams
        };

        var result = await _mediator.Send(request, cancellationToken);

        if (result.IsError) 
            return HandleErrorResponse(result.Errors);

        var branches = result.Payload?.Data ?? [];

        if (branches.Any())
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, 
                result.Payload.PageSize,
                result.Payload.TotalCount, 
                result.Payload.TotalPages);
        }

        return Ok(branches);
    }

    /// <summary>
    /// Retrieves a specific branch by ID.
    /// </summary>
    /// <param name="id">Branch ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Branch details</returns>
    [HttpGet(ApiRoutes.Branches.IdRoute)]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBranchById([FromRoute(Name = "id")] int branchId, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetBranchByIdRequest()
        {
            BranchId = branchId
        };

        return await HandleRequest(query, cancellationToken);
    }

    #endregion

    #region POST Operations

    /// <summary>
    /// Creates a new branch.
    /// Requires Admin role authorization.
    /// </summary>
    /// <param name="request">Branch creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created branch details</returns>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchRequest request, 
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateBranchCommand>(request);

        return await HandleRequest(command, cancellationToken);
    }

    #endregion

    #region PUT Operations

    /// <summary>
    /// Updates an existing branch.
    /// Requires Admin role authorization.
    /// </summary>
    /// <param name="id">Branch ID to update</param>
    /// <param name="request">Branch update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated branch details</returns>
    [HttpPut(ApiRoutes.Branches.IdRoute)]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBranch([FromRoute(Name = "id")] int branchId, 
        [FromBody] UpdateBranchRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (branchId <= 0)
            return HandleErrorResponse([new Error(ErrorCode.BadRequest, "Branch ID must be greater than zero")]);

        var command = _mapper.Map<UpdateBranchCommand>(request);
        command.BranchId = branchId;

        return await HandleRequest(command, cancellationToken);
    }

    #endregion

    #region DELETE Operations

    /// <summary>
    /// Deletes a branch.
    /// Requires Admin role authorization.
    /// </summary>
    /// <param name="id">Branch ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete(ApiRoutes.Branches.IdRoute)]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBranch([FromRoute(Name = "id")] int branchId, 
        CancellationToken cancellationToken = default)
    {
        if (branchId <= 0)
            return HandleErrorResponse([new Error(ErrorCode.BadRequest, 
                "Branch ID must be greater than zero")]);

        var command = new DeleteBranchCommand()
        {
            BranchId = branchId
        };

        return await HandleRequest(command, cancellationToken);
    }

    #endregion
}