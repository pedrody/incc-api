using System.Text.Json;

namespace InccApi.Extensions;

public static class PaginationHeaderExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, int currentPage, 
                                           int itemsPerPage, int totalItems, int totalPages,
                                           bool hasNext, bool hasPrevious)
    {
        var paginationHeader = new
        {
            currentPage,
            itemsPerPage,
            totalItems,
            totalPages,
            hasNext,
            hasPrevious
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader, options));
        response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");
    }
}
