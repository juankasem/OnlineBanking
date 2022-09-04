using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Constants;
using OnlineBanking.Application.Features.Branch.Commands;
using OnlineBanking.Application.Features.Branch.Queries;
using OnlineBanking.Application.Models.Branch.Requests;
using OnlineBanking.Application.Models.Branch.Responses;

namespace OnlineBanking.API.Controllers;

[Authorize(Roles = Roles.Administrator)]
public class BranchController : BaseApiController
{
    [HttpGet(ApiRoutes.Branches.All)]
    [ProducesResponseType(typeof(BranchListResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<BranchListResponse>> ListAllBranches(CancellationToken cancellationToken = default)
    {
        var query = new GetAllBranchesRequest();
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    [HttpGet(ApiRoutes.Branches.IdRoute)]
    [ProducesResponseType(typeof(BranchResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<BranchResponse>> GetBranchById([FromRoute] int id,
                                                                    CancellationToken cancellationToken = default)
    {
        var query = new GetBranchByIdRequest() { BranchId = id };
        var result = await _mediator.Send(query);

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