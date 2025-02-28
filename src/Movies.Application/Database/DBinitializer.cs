using Dapper;

namespace Movies.Application.Database;

public class DBInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DBInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync("""
            Create Table if not exists movies (
            id UUID primary key,
            title TEXT not null,
            slug TEXT not null,
            yearOfRelease integer not null);
            """);

        await connection.ExecuteAsync("""
            create unique index concurrently if not exists movies_slug_idx
            on movies
            using btree(slug);
            """);

        await connection.ExecuteAsync("""
            create table if not exists genres(
            movieId UUID references movies(id),
            genre TEXT not null);
            """);
    }
}
