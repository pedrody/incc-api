using InccApi.Models;

namespace InccApi.DTOs.Mappings;

public static class InccDTOExtesions
{
    public static IEnumerable<InccResponseDTO> ToDto(this IEnumerable<InccEntry> inccEntries)
    {
        var inccEntriesResponseDto = inccEntries.Select(e => new InccResponseDTO
        {
            MonthYear = e.ReferenceDate.ToString("MM/yyyy"),
            Value = e.Value
        }).ToList() ;

        return inccEntriesResponseDto;
    }
}
