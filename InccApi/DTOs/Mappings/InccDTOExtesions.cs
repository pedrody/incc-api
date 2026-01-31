using InccApi.Models;

namespace InccApi.DTOs.Mappings;

public static class InccDTOExtesions
{
    public static IEnumerable<InccResponseDTO> ToDtoList(this IEnumerable<InccEntry> inccEntries)
    {
        return inccEntries.Select(e => e.ToDto()).ToList();
    }

    public static InccResponseDTO ToDto(this InccEntry inccEntry)
    {
        return new InccResponseDTO
        {
            MonthYear = inccEntry.ReferenceDate.ToString("MM/yyyy"),
            Value = inccEntry.Value
        };
    }
}
