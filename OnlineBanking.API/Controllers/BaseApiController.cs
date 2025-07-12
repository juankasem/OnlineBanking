using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Common;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Models;

namespace OnlineBanking.API.Controllers;

[ApiController]
[Route(ApiRoutes.BaseRoute)]
[ApiVersion("1.0")]
public class BaseApiController : ControllerBase
{
    private IMediator _mediatorInstance;
    private IMapper _mapperInstance;

    protected IMediator _mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();
    protected IMapper _mapper => _mapperInstance ??= HttpContext.RequestServices.GetService<IMapper>();

    protected async Task<IActionResult> HandleRequest<TResponse>(IRequest<ApiResult<TResponse>> request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result.Payload);
    }

    protected IActionResult HandleErrorResponse(List<Error> errors)
    {
        var apiError = new ErrorResponse();

        if (errors.Any(e => e.Code == ErrorCode.NotFound))
        {
            var error = errors.FirstOrDefault(er => er.Code == ErrorCode.NotFound);

            if (error is not null)
            {
                apiError.StatusCode = (int)ErrorCode.NotFound;
                apiError.StatusPhrase = "Not Found";
                apiError.Timestamp = DateTime.UtcNow;
                apiError.Errors.Add(error.Message);
            }

            return NotFound(apiError);
        }

        if (errors.Any(e => e.Code == ErrorCode.CreateCashTransactionNotAuthorized))
        {
            var error = errors.FirstOrDefault(er => er.Code == ErrorCode.CreateCashTransactionNotAuthorized);

            if (error is not null)
            {
                apiError.StatusCode = 403;
                apiError.StatusPhrase = "Forbidden";
                apiError.Timestamp = DateTime.UtcNow;
                apiError.Errors.Add(error.Message);
            }

            return StatusCode(403, apiError);
        }

        if (errors.Any(e => e.Code == ErrorCode.InternalServerError ||
                            e.Code == ErrorCode.UnknownError))
        {
            var error = errors.FirstOrDefault(er => er.Code == ErrorCode.InternalServerError || 
                        er.Code == ErrorCode.UnknownError);

            if (error is not null)
            {
                apiError.StatusCode = (int)ErrorCode.InternalServerError;
                apiError.StatusPhrase = "Internal Server Error";
                apiError.Timestamp = DateTime.UtcNow;
                apiError.Errors.Add(error.Message);
            }

            return StatusCode(500, apiError);
        }

        apiError.StatusCode = (int)ErrorCode.BadRequest;
        apiError.StatusPhrase = "Bad Request";
        apiError.Timestamp = DateTime.UtcNow;
        errors.ForEach(er => apiError.Errors.Add(er.Message));

        return BadRequest(apiError);
    }
}