using FluentValidation;
using MediatR;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Common.Behaviors
{

    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<TRequest> _validator;

        public ValidationBehavior(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var result = new ApiResult<TResponse>();

            if (_validator is null)
            {
                return await next();
            }

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid)
            {
                return await next();
            }

            var errors = validationResult.Errors;

            errors.ForEach(er => result.AddError(ErrorCode.ValidationError, er.ErrorMessage));

            return (dynamic) result;
        }
    }
}