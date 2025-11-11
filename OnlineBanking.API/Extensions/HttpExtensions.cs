
namespace OnlineBanking.API.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerpage, int totalItems, int totalPages)
    {
        var paginationHeader = new
        {
            currentPage,
            itemsPerpage,
            totalItems,
            totalPages
        };

        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
        response.Headers.Add(HeaderNames.AccessControlExposeHeaders, "Pagination");
    }
}