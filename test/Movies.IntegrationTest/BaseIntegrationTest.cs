using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Service;

namespace Movies.IntegrationTest;

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly IMovieService _movieService;

    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _movieService = factory.Services.GetRequiredService<IMovieService>();
    }
    
    public void Dispose()
    {
        _scope.Dispose();
    }
    
}
