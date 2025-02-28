using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repository;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            INSERT INTO movies (id, title, slug , yearOfRelease)
            VALUES (@Id, @Title, @Slug,@YearOfRelease)
        """, movie, cancellationToken: token));

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    INSERT INTO genres (movieId, genre)
                    VALUES (@MovieId, @Name)
                """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
            }
        }

        transaction.Commit();

        return result > 0;


    }

    public Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {

        throw new NotImplementedException();
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();

    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            SELECT * FROM movies WHERE id = @Id
        """, new { Id = id }, cancellationToken: token));

        return movie is not null;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            SELECT * FROM movies WHERE id = @Id
        """, new { Id = id }));

        if (movie is null)
            return null;

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            SELECT genre FROM genres WHERE movieId = @id
        """, new { id = movie.Id }, cancellationToken: token));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            SELECT * FROM movies WHERE slug = @Slug
        """, new { Slug = slug }, cancellationToken: token));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            SELECT genre FROM genres WHERE movieId = @id
        """, new { id = movie.Id }, cancellationToken: token));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;

    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.QueryAsync(new CommandDefinition("""
                Select m.*, string_agg(g.genre, ',') as genres
                from movies  m left join genres  g on m.id = g.movieId
                group by id
                """, cancellationToken: token));

        return result.Select(r => new Movie
        {
            Id = r.id,
            Title = r.title,
            YearOfRelease = r.yearOfRelease,
            Genres = Enumerable.ToList(r.genre.Split(','))
        });
    }
}
