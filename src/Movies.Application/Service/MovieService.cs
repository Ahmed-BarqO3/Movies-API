using Movies.Application.Models;
using Movies.Application.Repository;

namespace Movies.Application.Service;

public class MovieService : IMovieService
{
    readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        return await _movieRepository.CreateAsync(movie, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        var existingMovie = await _movieRepository.ExistsByIdAsync(movie.Id, token);
        if (!existingMovie)
        {
            return null;

        }

        await _movieRepository.UpdateAsync(movie, token);
        return movie;
    }
    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _movieRepository.DeleteByIdAsync(id, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        return await _movieRepository.GetAllAsync(token);
    }

    public async Task<Movie> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _movieRepository.GetByIdAsync(id, token);
    }

    public async Task<Movie> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        return await _movieRepository.GetBySlugAsync(slug, token);
    }

}
