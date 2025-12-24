
namespace OnlineBanking.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that handles validation for MediatR requests.
/// Executes all validators for a request and aggregates errors if validation fails.
/// Implements cross-cutting concern for request validation without cluttering request handlers.
/// </summary>
/// <typeparam name="TRequest">The request type to validate</typeparam>
/// <typeparam name="TResponse">The response type (must be ApiResult<Unit>)</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ApiResult<Unit>, new()
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the ValidationBehavior.
    /// </summary>
    /// <param name="validators">Collection of validators for the request type</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the pipeline request by validating before delegating to the next handler.
    /// Aggregates all validation errors and returns them in the response if validation fails.
    /// </summary>
    /// <param name="request">The request to validate</param>
    /// <param name="next">The next pipeline handler delegate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response with validation errors or result from next handler</returns>
    public async Task<TResponse> Handle(TRequest request,
                                        RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(request);

        // If no validators are configured, proceed directly to next handler
        if (!_validators.Any())
            return await next();

        // Execute all validators concurrently
        var validationResult = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(request, cancellationToken)));

        // Aggregate all validation errors
        var validationErrors = validationResult
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // If validation passes, proceed to next handler
        if (validationErrors.Count == 0)
        {
            return await next();
        }

        // Create response with aggregated validation errors
        TResponse response = new();
        validationErrors.ForEach(error => response.AddError(ErrorCode.ValidationError, error.ErrorMessage));

        return response;
    }
}