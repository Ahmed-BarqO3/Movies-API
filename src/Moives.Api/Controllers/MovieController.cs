using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moives.Api.Mapping;
using Movies.Api.Auth;
using Movies.Application.Models;
using Movies.Application.Service;
using Movies.Contracts.Requsets;

namespace Movies.Api.Controllers;

[Authorize]
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
        var userid = HttpContext.GetUserId();
        var result = await _movieService.UpdateAsync(movie,userid,token);
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

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id,userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug,userId, token);

        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.MapToMovieResponse();
        return Ok(response);
    }



    [Authorize]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMovieRequest request,CancellationToken token)
    {        
        var userid = HttpContext.GetUserId();
        var options = request.MapToOptions()
            .WithUser(userid.Value);
        var count = await _movieService.GetCountAsync(request.Title, request.YearOfRelease, token);
        var movies = await _movieService.GetAllAsync(options,token);
        var response = movies.MapToMoviesResponse(request.Page, request.PageSize, count);

        return Ok(response);
    }

}
