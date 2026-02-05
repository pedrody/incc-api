using InccApi.DTOs;

namespace InccApi.Services;

public interface IInccService
{
    public Task<InccAccumulatedResponseDTO?> AccumulatedVariationAsync(
        InccAccumulatedParams @params);
}
