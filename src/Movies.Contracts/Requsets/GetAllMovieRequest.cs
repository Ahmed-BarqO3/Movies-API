namespace Movies.Contracts.Requsets;

public class GetAllMovieRequest
{
    public required string? Title { get; init; }
    public required int? YearOfRelease { get; init; }
    
}
