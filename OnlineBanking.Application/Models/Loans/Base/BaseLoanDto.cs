namespace OnlineBanking.Application.Models.Loans.Base;

public class BaseLoanDto
{
    public string CustomerId { get; set; }

    public string IBAN { get; set; }

    public Money Amount { get; set; }

    public decimal InterestRate { get; set; }

    public DateTime ClaimDate { get; set; }
}
