using FluentValidation;
using MediatR;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse: ApiResult<Unit>, new()
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, 
                                        RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        if (_validators.Any())
        {
            var validationResult = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(request, cancellationToken)));

            var errors = validationResult.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (errors.Count != 0)
            {
                TResponse response = new();

                errors.ForEach(error => response.AddError(Enums.ErrorCode.ValidationError, error.ErrorMessage));

                return await Task.FromResult(response);
            }
            else
            {
                return await next();
            }
        }

        return await next();
    }
}