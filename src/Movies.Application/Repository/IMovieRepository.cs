using Movies.Application.Models;

namespace Movies.Application.Repository;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie, CancellationToken token = default);

    Task<bool> UpdateAsync(Movie movie, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, Guid userid, CancellationToken token = default);

    Task<Movie?> GetByIdAsync(Guid id, Guid? Userid = default, CancellationToken token = default);

    Task<Movie?> GetBySlugAsync(string slug, Guid? Userid = default, CancellationToken token = default);

    Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default);

    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
    Task<int> GetCountAsync(string? title, int? yearofrelease, CancellationToken token = default);
}
