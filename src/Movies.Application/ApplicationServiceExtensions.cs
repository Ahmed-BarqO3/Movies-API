using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repository;
using Movies.Application.Service;

namespace Movies.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection service)
    {
        service.AddSingleton<IMovieRepository, MovieRepository>();
        service.AddSingleton<IMovieService, MovieService>();
        return service;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection service, string ConnectionString)
    {
        service.AddSingleton<IDbConnectionFactory>(_ => new
            NpgsqlConnectionFactory(ConnectionString));
        service.AddSingleton<DBInitializer>();
        return service;
    }
}
