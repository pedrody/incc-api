namespace InccApi.DTOs;

public class InccCreateDto
{
    public DateTime ReferenceDate { get; set; }
    public decimal Value { get; set; }
    public double MonthlyVariation { get; set; }
}
