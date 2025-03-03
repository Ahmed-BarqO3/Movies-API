using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validator;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x=>x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.Now.Year);
    }
    
}
