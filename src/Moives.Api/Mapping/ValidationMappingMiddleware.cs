using FluentValidation;
using Movies.Contracts.Responses;

namespace Moives.Api.Mapping;

 
public class ValidationMappingMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMappingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException e)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var validationErrors = new ValidationFailureResponse
            {
                Errors = e.Errors.Select(x=> new ValidationResponse
                {
                    PropertyName = x.PropertyName,
                    ErrorMessage = x.ErrorMessage
                })
            };
            await context.Response.WriteAsJsonAsync(validationErrors);
        }
        
    }
}
