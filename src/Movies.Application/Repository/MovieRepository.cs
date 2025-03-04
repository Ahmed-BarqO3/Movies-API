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
                    INSERT INTO genres (movieId, name)
                    VALUES (@MovieId, @Name)
                """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
            }
        }

        transaction.Commit();

        return result > 0;


    }

    public async Task<bool> UpdateAsync(Movie movie , CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                Delete from genres where movieId = @Id
                """, new { movie.Id }, cancellationToken: token));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO genres (movieId, name)
                VALUES (@MovieId, @Name)
            """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            UPDATE movies
            SET title = @Title, slug = @Slug, yearOfRelease = @YearOfRelease
            WHERE id = @Id
        """, movie, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        
        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
                         DELETE FROM genres WHERE movieId = @Id
                        """, new { Id = id }, cancellationToken: token));
        
        if(result > 0)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                  DELETE FROM movies WHERE id = @Id
                  """, new { Id = id }, cancellationToken: token));
        }
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            SELECT  COUNT(*) From movies WHERE id = @id
        """, new { id }, cancellationToken: token));

        return result;
    }

    public async Task<Movie?> GetByIdAsync(Guid id,Guid? Userid = default, CancellationToken token = default)
    {

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            SELECT m.*, round(avg(r.rating),1) as rating, myr.rating as userrating
                from movies
                left join ratings r on m.id = r.movieid
                left join ratings myr on m.id = myr.movieid
                and myr.userid = @Userid
                where id = @id
                group by id,userrating
        """, new { id, Userid }, cancellationToken: token));

        if (movie is null)
            return null;

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            SELECT name FROM genres WHERE movieId = @id
        """, new { id = movie.Id }, cancellationToken: token));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug,Guid? Userid = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            SELECT m.*, round(avg(r.rating),1) as rating, myr.rating as userrating
            from movies m
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id = myr.movieid
            and myr.userid = @Userid
            where slug = @Slug
            group by id,userrating
        """, new { Slug = slug, Userid }, cancellationToken: token));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            SELECT name FROM genres WHERE movieId = @id
        """, new { id = movie.Id }, cancellationToken: token));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;

    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options,CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        
        var OrderClause = string.Empty;
        if (options.SortField is not null)
        {
            OrderClause = $"""
            , m.{options.SortField} 
                ORDER BY m.{options.SortField} {(options.SortOrder == SortOrder.Descending ? "DESC" : "ASC")}
            """;
        }
        var result = await connection.QueryAsync(new CommandDefinition($"""
                Select m.*,
                    string_agg( distinct g.name, ',') as genres,
                    round(avg(r.rating),1) as rating, myr.rating as userrating
                    from movies m
                    left join genres g on m.id = g.movieId
                    left join ratings r on m.id = r.movieid
                    left join ratings myr on m.id = myr.movieid
                    and myr.userid = @Userid
                    
                    where (@title is null or m.title like ('%' || @title || '%'))
                    and (@year is null or m.yearOfRelease = @year)
                
                    group by id,userrating {OrderClause}
                """,new
        {
            Userid = options.UserId,
            year = options.YearOfRelease,
            title = options.Title
        }, cancellationToken: token));

        return result.Select(r => new Movie
        {
            Id = r.id,
            Title = r.title,
            YearOfRelease = r.yearofrelease,
            Rating = (float?)r.rating,
            UserRating = (int?)r.userrating,
            Genres = Enumerable.ToList(r.genres.Split(','))
        });
    }
}
