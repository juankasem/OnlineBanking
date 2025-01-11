using FluentValidation;
using MediatR;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineBanking.Application.Common.Behaviors;

    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var result = new ApiResult<TResponse>();

            if (_validators.Any())
            {
                var validationResult = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(request, cancellationToken)));

                var errors = validationResult.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (!errors.Any())
                {
                    return await next();
                }

                errors.ForEach(er => result.AddError(ErrorCode.ValidationError, er.ErrorMessage));

                 return (dynamic)result;
            }

            return await next();
        }
    }