using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;
using Movies.Application.Repository;
using Movies.Contracts.Responses;

namespace Movies.Application.Service;

public class RatingService : IRatingService
{
    IMovieRepository _movieRepository;
    IRatingRepository _ratingRepository;

    public RatingService(IMovieRepository movieRepository, IRatingRepository ratingRepository)
    {
        _movieRepository = movieRepository;
        _ratingRepository = ratingRepository;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, Guid userid, int rating, CancellationToken token = default)
    {
        if (rating is < 1 or > 5)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure { PropertyName = "Rating", ErrorMessage = "Rating must be between 1 and 5" }
            });
        }

        var existsMovie = await _movieRepository.ExistsByIdAsync(movieId, token);
        if (!existsMovie)
        {
            return false;
        }

        return await _ratingRepository.RateMovieAsync(movieId, userid, rating, token);
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        return await _ratingRepository.DeleteRatingAsync(movieId, userId, token);
    }

    public async Task<IEnumerable<MovieRating>> GetUserRatingsAsync(Guid userId, CancellationToken token = default)
    {
        return await _ratingRepository.GetUserRatingsAsync(userId, token);
    }
}
