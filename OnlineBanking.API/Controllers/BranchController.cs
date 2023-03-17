using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Constants;
using OnlineBanking.API.Extensions;
using OnlineBanking.API.Helpers;
using OnlineBanking.Application.Features.Branch.Commands;
using OnlineBanking.Application.Features.Branch.Queries;
using OnlineBanking.Application.Models.Branch.Requests;
using OnlineBanking.Application.Models.Branch.Responses;
using OnlineBanking.Core.Helpers;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.API.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class BranchController : BaseApiController
{
    [Cached(600)]
    [HttpGet(ApiRoutes.Branches.All)]
    [ProducesResponseType(typeof(PagedList<BranchResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PagedList<BranchResponse>>> ListAllBranches([FromQuery] BranchParams branchParams ,CancellationToken cancellationToken = default)
    {
        var request = new GetAllBranchesRequest() { BranchParams = branchParams };
        var result = await _mediator.Send(request, cancellationToken);

        if (result.IsError) HandleErrorResponse(result.Errors);

        Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                                    result.Payload.TotalCount, result.Payload.TotalPages);

        return Ok(result.Payload);
    }

    [Cached(600)]
    [HttpGet(ApiRoutes.Branches.IdRoute)]
    [ProducesResponseType(typeof(BranchResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<BranchResponse>> GetBranchById([FromRoute] int id,
                                                                CancellationToken cancellationToken = default)
    {
        var request = new GetBranchByIdRequest() { BranchId = id };
        var result = await _mediator.Send(request);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchRequest request,
                                                    CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateBranchCommand>(request);
        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    [HttpPut(ApiRoutes.Branches.IdRoute)]
    public async Task<IActionResult> UpdateBranch([FromRoute] int id, [FromBody] UpdateBranchRequest request,
                                                    CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateBranchCommand>(request);
        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    [HttpDelete(ApiRoutes.Branches.IdRoute)]
    public async Task<IActionResult> DeleteBranch([FromRoute] int id,
                                                CancellationToken cancellationToken = default)
    {
        var command = new DeleteBranchCommand() { BranchId = id };
        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }
}