namespace Movies.Contracts.Requsets;

public class PagedRequset
{
    public int PageSize { get; init; } = 10;
    public int Page { get; init; } = 1;
}
