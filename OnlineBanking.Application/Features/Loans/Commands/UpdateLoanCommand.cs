using MediatR;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.Loans.Commands;

public class UpdateLoanCommand : IRequest<ApiResult<Unit>>
{

}