using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validator;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] _allowedSortBy = ["title", "yearofrelease"];
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x=>x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.Now.Year);

        RuleFor(x => x.SortField)
            .Must(x => x is null || _allowedSortBy.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid sort field. Allowed values are: title, yearofrelease");
    }
}
