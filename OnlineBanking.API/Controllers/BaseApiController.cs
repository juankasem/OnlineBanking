
namespace OnlineBanking.API.Controllers;

/// <summary>
/// Base controller providing common functionality for API controllers.
/// Handles request/response processing and error responses with appropriate HTTP status codes.
/// </summary>
[ApiController]
[Route(ApiRoutes.BaseRoute)]
[ApiVersion("1.0")]
public class BaseApiController : ControllerBase
{
    private IMediator _mediatorInstance;
    private IMapper _mapperInstance;

    /// <summary>
    /// Lazily initializes and returns the MediatR mediator instance.
    /// </summary>
    protected IMediator _mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();

    /// <summary>
    /// Lazily initializes and returns the AutoMapper instance.
    /// </summary>
    protected IMapper _mapper => _mapperInstance ??= HttpContext.RequestServices.GetService<IMapper>();

    /// <summary>
    /// Handles a request and returns an appropriate HTTP response.
    /// </summary>
    /// <typeparam name="TResponse">Response payload type</typeparam>
    /// <param name="request">The request to send through the mediator</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>OK response with payload or error response</returns>
    protected async Task<IActionResult> HandleRequest<TResponse>(IRequest<ApiResult<TResponse>> request, 
                                                      CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result.Payload);
    }

    /// <summary>
    /// Handles error responses and maps error codes to appropriate HTTP status codes.
    /// </summary>
    /// <param name="errors">List of errors to process</param>
    /// <returns>IActionResult with appropriate HTTP status code</returns>
    protected IActionResult HandleErrorResponse(List<Error> errors)
    {
        if (errors.Count == 0)
            return BadRequest(CreateErrorResponse(StatusCodes.Status400BadRequest, ErrorPhrase.BadRequest, []));

        // Determine response based on first error's code (priority-based)
        var (statusCode, statusPhrase, errorMessages) = errors.FirstOrDefault()?.Code switch
        {
            ErrorCode.BadRequest =>
              (
                  StatusCodes.Status400BadRequest,
                  ErrorPhrase.BadRequest,
                  errors.Where(e => e.Code == ErrorCode.NotFound)
                         .Select(e => e.Message)
                         .ToList()
              ),

            ErrorCode.NotFound =>
                (
                    StatusCodes.Status404NotFound,
                    ErrorPhrase.NotFound,
                    errors.Where(e => e.Code == ErrorCode.NotFound)
                           .Select(e => e.Message)
                           .ToList()
                ),

            ErrorCode.CreateCashTransactionNotAuthorized or ErrorCode.UnAuthorizedOperation =>
                (
                    StatusCodes.Status403Forbidden,
                    ErrorPhrase.Forbidden,
                    errors.Where(e => e.Code == ErrorCode.CreateCashTransactionNotAuthorized ||
                                      e.Code == ErrorCode.UnAuthorizedOperation)
                           .Select(e => e.Message)
                           .ToList()
                ),

            ErrorCode.InSufficintFunds =>
                (
                    StatusCodes.Status400BadRequest,
                    ErrorPhrase.InsufficientFunds,
                    errors.Where(e => e.Code == ErrorCode.InSufficintFunds)
                           .Select(e => e.Message)
                           .ToList()
                ),

            ErrorCode.InternalServerError or ErrorCode.UnknownError =>
                (
                    StatusCodes.Status500InternalServerError,
                    ErrorPhrase.InternalServerError,
                    errors.Where(e => e.Code == ErrorCode.InternalServerError ||
                                     e.Code == ErrorCode.UnknownError)
                           .Select(e => e.Message)
                           .ToList()
                ),

            _ =>
                (
                    StatusCodes.Status400BadRequest,
                    ErrorPhrase.BadRequest,
                    errors.Select(e => e.Message).ToList()
                )
        };

        var errorResponse = CreateErrorResponse(statusCode, statusPhrase, errorMessages);
        return StatusCode(statusCode, errorResponse);
    }

    /// <summary>
    /// Creates an error response object with timestamp and error details.
    /// </summary>
    private static ErrorResponse CreateErrorResponse(int statusCode, string statusPhrase, List<string> errorMessages)
    {
        var errorResponse = new ErrorResponse
        {
            StatusCode = statusCode,
            StatusPhrase = statusPhrase,
            Timestamp = DateTime.UtcNow
        };

        errorMessages.ForEach(msg => errorResponse.Errors.Add(msg));
        return errorResponse;
    }
}