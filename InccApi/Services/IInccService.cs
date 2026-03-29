using InccApi.DTOs;
using InccApi.Pagination;

namespace InccApi.Services;

public interface IInccService
{
    Task<PagedList<InccResponseDTO>> GetPaginatedAsync(PaginationParams paginationParams);
    Task<InccResponseDTO?> GetByDateAsync(int year, int month);
    Task<PagedList<InccResponseDTO>> GetRangeAsync(InccRangeParams @params);
    Task<InccAccumulatedResponseDTO?> AccumulatedVariationAsync(
        InccAccumulatedParams @params);
    Task<InccResponseDTO?> Create(InccCreateDto entry);
}
