using OnlineBanking.Application.Models.Loans.Responses;

namespace OnlineBanking.Application.Features.Loans.GetAll;

public class GetAllLoansRequest : IRequest<ApiResult<LoanListResponse>>
{

}