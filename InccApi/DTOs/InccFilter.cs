using InccApi.Pagination;
using System.ComponentModel.DataAnnotations;

namespace InccApi.DTOs;

public class InccFilter : PaginationParams
{
    [Required(ErrorMessage = "Necessário informar o ano de início")]
    [Range(1995, 2100)]
    public int? StartYear { get; set; }

    [Required(ErrorMessage = "Necessário informar o mês de início")]
    [Range(1, 12, ErrorMessage = "O mês deve estar entre 1 e 12.")]
    public int? StartMonth { get; set; }

    [Range(1995, 2100)]
    public int? EndYear { get; set; }

    [Range(1, 12, ErrorMessage = "O mês deve estar entre 1 e 12.")]
    public int? EndMonth { get; set; }

    
    public DateTime? GetStartDate()
    {
        if (!StartYear.HasValue)
        {
            return null;
        }

        int month = StartMonth ?? 1;

        return new DateTime(StartYear.Value, month, 1);
    }

    public DateTime? GetEndDate()
    {
        if (!EndYear.HasValue)
        {
            return null;
        }

        int month = EndMonth ?? 12;

        return new DateTime(EndYear.Value, month, 1);
    }

    public bool IsValid()
    {
        var start = GetStartDate();
        var end = GetEndDate();

        if (start == null || end == null)
        {
            return true;
        }

        return start <= end;
    }
}
