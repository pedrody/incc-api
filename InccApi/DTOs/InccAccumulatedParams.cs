using System.ComponentModel.DataAnnotations;

namespace InccApi.DTOs;

public class InccAccumulatedParams
{
    public decimal? Amount { get; set;  }

    [Required(ErrorMessage = "Necessário informar o ano de início")]
    [Range(1995, 2100)]
    public int StartYear { get; set; }

    [Required(ErrorMessage = "Necessário informar o mês de início")]
    [Range(1, 12, ErrorMessage = "O mês deve estar entre 1 e 12.")]
    public int StartMonth { get; set; }

    [Required(ErrorMessage = "Necessário informar o ano de fim")]
    [Range(1995, 2100)]
    public int EndYear { get; set; }

    [Required(ErrorMessage = "Necessário informar o mês de fim")]
    [Range(1, 12, ErrorMessage = "O mês deve estar entre 1 e 12.")]
    public int EndMonth { get; set; }


    public DateTime GetStartDate()
    {
        return new DateTime(StartYear, StartMonth, 1);
    }

    public DateTime GetEndDate()
    {
        return new DateTime(EndYear, EndMonth, 1);
    }

    public bool IsValid()
    {
        var start = GetStartDate();
        var end = GetEndDate();

        return start <= end;
    }
}
