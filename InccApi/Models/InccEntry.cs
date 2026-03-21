namespace InccApi.Models;

public class InccEntry
{
    public int Id { get; set; }
    public DateTime ReferenceDate { get; set; }
    public decimal Value { get; set; }
    public double MonthlyVariation { get; set; }
}
