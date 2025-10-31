
namespace OnlineBanking.Application.Models.Currency;

public class CurrencyDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }

    private CurrencyDto()
    {
    }

    public CurrencyDto(int id, string code, string name, string symbol)
    {
        Id = id;
        Code = code;
        Name = name;
        Symbol = symbol;
    }
}