using Moives.Api.Mapping;
using Movies.Application.Models;
using Movies.Contracts.Requsets;
using Shouldly;

namespace Movies.IntegrationTest;

public class MovieTests : BaseIntegrationTest
{
    public MovieTests(IntegrationTestWebAppFactory factory) : base(factory){}
    
    
    [Fact]
    public async Task GetMovies_ShouldReturnMovies()
    {
        // Arrange
        var request = new CreateMovieRequest()
        {
            Title = "The Matrix",
            YearOfRelease = 1999,
            Genres = ["Action", "Sci-Fi"]
        };

        var movie = request.MapToMovie();
       
        await _movieService.CreateAsync(movie);
        
        // Act
        var response = await _movieService.GetByIdAsync(movie.Id);

        // Assert
        response.ShouldNotBeNull();
        response.Id.ShouldBeEquivalentTo(movie.Id);
    }
}
