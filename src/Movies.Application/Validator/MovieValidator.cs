using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repository;

namespace Movies.Application.Validator;

public class MovieValidator : AbstractValidator<Movie>
{
    readonly IMovieRepository _movieRepository;

    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.Now.Year);
        
        RuleFor(x=>x.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("The Movie is already exists in the system");
    }

    private async Task<bool> ValidateSlug(Movie movie,string slug, CancellationToken token = default)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug, token: token);
        if (existingMovie is not null)
        {
          return  existingMovie.Id == movie.Id;
        }
        return existingMovie is null;
    }
}
