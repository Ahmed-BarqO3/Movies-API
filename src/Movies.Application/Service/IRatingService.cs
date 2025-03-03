using Movies.Application.Models;
using Movies.Contracts.Responses;

namespace Movies.Application.Service;

public interface IRatingService
{
    Task<bool> RateMovieAsync(Guid movieId, Guid userid, int rating, CancellationToken token = default);
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);
    Task<IEnumerable<MovieRating>> GetUserRatingsAsync(Guid userId, CancellationToken token = default);

}
