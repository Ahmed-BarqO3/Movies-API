using Dapper;

namespace Movies.Application.Database;

public class DBInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DBInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync(CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);


        await connection.ExecuteAsync("""
            Create Table if not exists movies (
            id UUID primary key,
            title TEXT not null,
            slug TEXT not null,
            yearofrelease integer not null);
            """);

        await connection.ExecuteAsync("""
            create unique index concurrently if not exists movies_slug_idx
            on movies
            using btree(slug);
            """);

        await connection.ExecuteAsync("""
            create table if not exists genres(
            movieId UUID references movies(id),
            name TEXT not null);
            """);

        await connection.ExecuteAsync("""
                                      create table if not exists ratings(
                                          userid uuid ,
                                          movieid uuid references movies(id),
                                          rating integer not null,
                                          primary key(userid,movieid));
                                      """);
    }
}
