using Dapper;
using Movies.Application.Database;

namespace Movies.Application.Repository;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QueryFirstOrDefaultAsync<float>(new CommandDefinition("""
            SELECT round(AVG(rating),1)
            FROM ratings
            WHERE movieId = @movieId
        """, new {  movieId }, cancellationToken: token));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
                            return await connection.QueryFirstOrDefaultAsync<(float?,int?)>(new CommandDefinition("""
                            SELECT round(AVG(rating),1),
                                (select rating
                                from ratings
                                where movieid = @movieId
                                    and userid = @userId
                                limit 1)
                            from ratings
                            where movieId = @movieId
                        """, new {  movieId,userId }, cancellationToken: token));
    }
}
