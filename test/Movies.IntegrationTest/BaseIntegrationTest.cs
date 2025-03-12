using System.Net.Http.Json;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api;
using Movies.Contracts.Requsets;

namespace Movies.IntegrationTest;

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly HttpClient _client;
    protected readonly Faker<CreateMovieRequest> _movieRequestFaker;
    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _client = factory.CreateClient();
        
        _movieRequestFaker = new Faker<CreateMovieRequest>()
            .RuleFor(m => m.Title, f => f.Lorem.Sentence(3))
            .RuleFor(m => m.YearOfRelease, f => f.Date.Past(10).Year)
            .RuleFor(m => m.Genres, f => f.PickRandom(Genres.All, 2).ToList());
    }
    
     static class Genres
    {
        public static readonly string[] All = 
        {
            "Action", "Comedy", "Drama", "Horror", "Sci-Fi",
            "Thriller", "Romance", "Documentary", "Animation"
        };
    }
    
    public void Dispose()
    {
        _scope.Dispose();
        _client.Dispose();
    }
    
}
