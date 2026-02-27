using InccApi.Pagination;
using System.ComponentModel.DataAnnotations;

namespace InccApi.DTOs;

public class InccRangeParams : PaginationParams
{
    [Required]
    [Range(1994, 2026)]
    public int? StartYear { get; set; }

    [Required]
    [Range(1, 12)]
    public int? StartMonth { get; set; }

    [Range(1994, 2026)]
    public int? EndYear { get; set; }

    [Range(1, 12)]
    public int? EndMonth { get; set; }
    
    public DateTime GetStartDate() => new(
        StartYear.Value, StartMonth.Value, 1);

    public DateTime? GetEndDate()
    {
        if (!EndYear.HasValue)
            return null;

        int month = EndMonth ?? 12;

        return new DateTime(EndYear.Value, month, 1);
    }

    public bool IsValid()
    {
        var start = GetStartDate();
        var end = GetEndDate();

        if (end == null)
            return true;

        return start <= end;
    }
}
