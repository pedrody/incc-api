namespace InccApi.DTOs;

public class InccPagedResponseDTO<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public bool HasNext => CurrentPage < TotalPages;
    public bool HasPrevious => CurrentPage > 1;
}
