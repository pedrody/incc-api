using InccApi.DTOs;
using InccApi.DTOs.Mappings;
using InccApi.Pagination;
using InccApi.Repositories;

namespace InccApi.Services;

public class InccService : IInccService
{
    private readonly IInccRepository _inccRepository;

    public InccService(IInccRepository repository)
    {
        _inccRepository = repository;
    }

    public async Task<InccAccumulatedResponseDTO?> AccumulatedVariationAsync(
        InccAccumulatedParams @params)
    {
        var inccStart = await _inccRepository.GetByDateAsync(@params.StartYear, @params.StartMonth);
        var inccEnd = await _inccRepository.GetByDateAsync(@params.EndYear, @params.EndMonth);

        if (inccStart == null || inccEnd == null)
            return null;

        if (inccStart.Value == 0)
            throw new ArgumentException("The initial INCC value cannot be zero.");

        var accumulated = (inccEnd.Value / inccStart.Value - 1) * 100;
        
        decimal? adjustedValue = null;
        if (@params.Amount.HasValue)
        {
            decimal factor = 1 + (accumulated / 100);
            adjustedValue = Math.Round(@params.Amount.Value * factor, 4);
        }

        return new InccAccumulatedResponseDTO
        {
            AccumulatedVariation = (double)Math.Round(accumulated, 4),
            AdjustedValue = adjustedValue,
            StartDate = inccStart.ReferenceDate.ToString("MM/yyyy"),
            EndDate = inccEnd.ReferenceDate.ToString("MM/yyyy")
        };
    }

    public async Task<InccResponseDTO?> GetByDateAsync(int year, int month)
    {
        var entry = await _inccRepository.GetByDateAsync(year, month);

        return entry?.ToResponseDto();
    }

    public async Task<PagedList<InccResponseDTO>> GetPaginatedAsync(PaginationParams paginationParams)
    {
        var entries = await _inccRepository.GetPaginatedAsync(paginationParams);

        return new PagedList<InccResponseDTO>(
                entries.Items.ToResponseDtoList(),
                entries.TotalCount,
                entries.CurrentPage,
                entries.PageSize
            );
    }

    public async Task<PagedList<InccResponseDTO>> GetRangeAsync(InccRangeParams @params)
    {
        var startDate = @params.GetStartDate();
        var endDate = @params.GetEndDate();

        var entries = await _inccRepository.GetRangeAsync(@params, startDate, endDate);

        return new PagedList<InccResponseDTO>(
                entries.Items.ToResponseDtoList(),
                entries.TotalCount,
                entries.CurrentPage,
                entries.PageSize
            );
    }

    public async Task<InccResponseDTO?> Create(InccCreateDto createEntry)
    {
        var normalizedDate = new DateTime(
            createEntry.ReferenceDate!.Value.Year,
            createEntry.ReferenceDate!.Value.Month, 
            1);
        
        if (await _inccRepository.GetByDateAsync(normalizedDate.Year, 
            normalizedDate.Month) != null)
        {
            return null;
        }

        var inccEntry = createEntry.ToInccEntry();
        inccEntry.ReferenceDate = normalizedDate;

        var inccEntryDb = await _inccRepository.Create(inccEntry);

        return inccEntryDb.ToResponseDto();
    }
}
