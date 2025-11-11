using FluentValidation;
using MediatR.Pipeline;


namespace OnlineBanking.Application.Common.Processors;

public class RequestValidationProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : IValidatableRequest
{
    private readonly IValidator<TRequest> _validator;

    public RequestValidationProcessor(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

    }
}

