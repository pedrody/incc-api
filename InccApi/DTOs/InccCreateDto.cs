using System.ComponentModel.DataAnnotations;

namespace InccApi.DTOs;

public class InccCreateDto
{
    [Required]
    public DateTime? ReferenceDate { get; set; }
    
    [Required]
    public decimal? Value { get; set; }

    [Required]
    public double? MonthlyVariation { get; set; }
}
