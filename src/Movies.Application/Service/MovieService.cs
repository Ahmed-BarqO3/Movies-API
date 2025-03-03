using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repository;

namespace Movies.Application.Service;

public class MovieService : IMovieService
{
    readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _movieValidator;
    private readonly IRatingRepository _ratingRepository;

    public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator, IRatingRepository ratingRepository)
    {
        _movieRepository = movieRepository;
        _movieValidator = movieValidator;
        _ratingRepository = ratingRepository;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);
        return await _movieRepository.CreateAsync(movie, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie,Guid? Userid = default, CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);
        var existingMovie = await _movieRepository.ExistsByIdAsync(movie.Id, token);
        if (!existingMovie)
        {
            return null;

        }
        await _movieRepository.UpdateAsync(movie, token);

        if (!Userid.HasValue)
        {
            movie.Rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
        }
        else
        {
            var ratings = await _ratingRepository.GetRatingAsync(movie.Id,Userid.Value , token);
            movie.Rating = ratings.Rating;
            movie.UserRating = ratings.UserRating;
        }
        return movie;
    }
    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _movieRepository.DeleteByIdAsync(id, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(Guid? Userid = default,CancellationToken token = default)
    {
        return await _movieRepository.GetAllAsync(Userid,token);
    }

    public async Task<Movie?> GetByIdAsync(Guid id,Guid? Userid=default, CancellationToken token = default)
    {
        return await _movieRepository.GetByIdAsync(id,Userid, token);
    }

    public async Task<Movie?> GetBySlugAsync(string slug,Guid? Userid = default, CancellationToken token = default)
    {
        return await _movieRepository.GetBySlugAsync(slug,Userid, token);
    }

}
