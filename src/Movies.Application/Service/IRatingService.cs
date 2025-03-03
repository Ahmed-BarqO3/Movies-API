namespace Movies.Application.Service;

public interface IRatingService
{
    Task<bool> RateMovieAsync(Guid movieId,Guid userid, int rating, CancellationToken token = default);
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);
}
