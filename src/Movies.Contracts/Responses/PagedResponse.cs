namespace Movies.Contracts.Responses;

public class PagedResponse<TResonse>
{
    public IEnumerable<TResonse> Items { get; set; } = Enumerable.Empty<TResonse>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public bool HasNextPage => Total > (Page * PageSize);
    
}
