using MediatR;


namespace OnlineBanking.Application.Common.Processors;

public interface IValidatableRequest<out TResponse> 
    : IRequest<TResponse>, IValidatableRequest { }

public interface IValidatableRequest { }