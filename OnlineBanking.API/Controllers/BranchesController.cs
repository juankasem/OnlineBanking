using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Constants;
using OnlineBanking.API.Extensions;
using OnlineBanking.Application.Features.Branch.Commands;
using OnlineBanking.Application.Features.Branch.Queries;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models.Branch.Requests;
using OnlineBanking.Application.Models.Branch.Responses;

namespace OnlineBanking.API.Controllers;

[Authorize]
public class BranchesController : BaseApiController
{
    [HttpGet(ApiRoutes.Branches.All)]
    [ProducesResponseType(typeof(PagedList<BranchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAllBranches([FromQuery] BranchParams branchParams, CancellationToken cancellationToken = default)
    {
        var request = new GetAllBranchesRequest()
        {
            BranchParams = branchParams
        };

        var result = await _mediator.Send(request, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        var branches = result.Payload.Data;

        if (branches.Any())
        {
            Response.AddPaginationHeader(result.Payload.CurrentPage, result.Payload.PageSize,
                        result.Payload.TotalCount, result.Payload.TotalPages);
        }

        return Ok(branches);
    }

    [HttpGet(ApiRoutes.Branches.IdRoute)]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBranchById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var request = new GetBranchByIdRequest() { BranchId = id };
        var result = await _mediator.Send(request, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }


    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchRequest request, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateBranchCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok();
    }

    [HttpPut(ApiRoutes.Branches.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBranch([FromRoute] int id,
                                                  [FromBody] UpdateBranchRequest request,
                                                  CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateBranchCommand>(request);
        command.BranchId = id;

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok();
    }

    [HttpDelete(ApiRoutes.Branches.IdRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteBranch([FromRoute] int id,
                                                  CancellationToken cancellationToken = default)
    {
        var command = new DeleteBranchCommand()
        {
            BranchId = id
        };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok();
    }
}