namespace Movies.Application.Models;

public class GetAllMoviesOptions
{
    public  Guid? UserId { get; set; }
    public required string? Title { get; init; }
    public required int? YearOfRelease { get; init; }
}
