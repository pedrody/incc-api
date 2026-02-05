using InccApi.DTOs;
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
        {
            return null;
        }

        var accumulated = ((double)inccEnd.Value / (double)inccStart.Value - 1) * 100;
        
        decimal? adjustedValue = null;
        if (@params.Amount.HasValue)
        {
            decimal factor = 1 + ((decimal)accumulated / 100);
            adjustedValue = Math.Round(@params.Amount.Value * factor, 4);
        }

        return new InccAccumulatedResponseDTO
        {
            AccumulatedVariation = Math.Round(accumulated, 4),
            AdjustedValue = adjustedValue,
            StartDate = inccStart.ReferenceDate.ToString("MM/yyyy"),
            EndDate = inccEnd.ReferenceDate.ToString("MM/yyyy")
        };
    }
}
