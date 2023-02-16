namespace OnlineBanking.Application.Models.Address.Base;

public class BaseAddressDto
{
    public string Name { get; set; }
    public string ZipCode { get; set; }
    public int DistrictId { get; set; }
    public int CityId { get; set; }
    public int CountryId { get; set; }
    public string Street { get; set; }
}
