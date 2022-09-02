using System.Collections.Generic;

namespace OnlineBanking.Application.Models.Customer.Responses;

public class CustomerListResponse
{
  public List<CustomerResponse> Customers { get; set; }

  public int Count { get; set; }
}
