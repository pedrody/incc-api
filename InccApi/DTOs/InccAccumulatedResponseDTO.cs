namespace InccApi.DTOs;

public class InccAccumulatedResponseDTO
{
    public double AccumulatedVariation { get; set; }
    public decimal? AdjustedValue { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
}
