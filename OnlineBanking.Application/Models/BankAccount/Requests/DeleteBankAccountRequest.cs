using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Application.Models.BankAccount.Requests;

public class DeleteBankAccountRequest
{
   public int AccountId { get; set; }
}
