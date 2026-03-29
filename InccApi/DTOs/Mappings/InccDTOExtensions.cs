using InccApi.Models;

namespace InccApi.DTOs.Mappings;

public static class InccDTOExtensions
{
    public static IEnumerable<InccResponseDTO> ToResponseDtoList(this IEnumerable<InccEntry> inccEntries)
    {
        return inccEntries.Select(e => e.ToResponseDto()).ToList();
    }

    public static InccResponseDTO ToResponseDto(this InccEntry inccEntry)
    {
        return new InccResponseDTO
        {
            MonthYear = inccEntry.ReferenceDate.ToString("MM/yyyy"),
            Value = inccEntry.Value,
            MonthlyVariation = inccEntry.MonthlyVariation
        };
    }
}
