using System.ComponentModel.DataAnnotations;

namespace InccApi.DTOs;

public class InccAccumulatedParams : IValidatableObject
{
    public decimal? Amount { get; set;  }

    [Required]
    [Range(1994, 2026)]
    public int StartYear { get; set; }

    [Required]
    [Range(1, 12)]
    public int StartMonth { get; set; }

    [Required]
    [Range(1994, 2026)]
    public int EndYear { get; set; }

    [Required]
    [Range(1, 12)]
    public int EndMonth { get; set; }

    public DateTime GetStartDate() => new(StartYear, StartMonth, 1);

    public DateTime GetEndDate() => new(EndYear, EndMonth, 1);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (GetStartDate() > GetEndDate())
        {
            yield return new ValidationResult(
                "Start date can't be greater than end date",
                new[]
                {
                    nameof(StartMonth),
                    nameof(StartYear),
                    nameof(EndMonth),
                    nameof(EndYear)
                });
        }
    }
}
