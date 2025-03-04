namespace Movies.Application.Models;

public class GetAllMoviesOptions
{
    public  Guid? UserId { get; set; }
    public required string? Title { get; init; }
    public required int? YearOfRelease { get; init; }
    
    public string? SortField { get; set; }
    public SortOrder? SortOrder { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; }
}

public enum SortOrder
{
    Unsorted,
    Descending,
    Ascending
}
