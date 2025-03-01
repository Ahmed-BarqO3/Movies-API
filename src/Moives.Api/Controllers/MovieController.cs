using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moives.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Service;
using Movies.Contracts.Requsets;

namespace Movies.Api.Controllers;

[ApiController]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;

    }


    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie();
        return await _movieService.CreateAsync(movie, token)
            ? CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie)
            : BadRequest();
    }


    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie(id);
        var result = await _movieService.UpdateAsync(movie, token);
        if (result is null)
        {
            return NotFound();
        }
        var response = movie.MapToMovieResponse();
        return Ok(response);

    }
    
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        return await _movieService.DeleteByIdAsync(id, token)
            ? Ok()
            : NotFound();
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, token)
            : await _movieService.GetBySlugAsync(idOrSlug, token);

        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.MapToMovieResponse();
        return Ok(response);
    }




    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var movies = await _movieService.GetAllAsync(token);
        var response = movies.MapToMoviesResponse();

        return Ok(response);
    }

}
