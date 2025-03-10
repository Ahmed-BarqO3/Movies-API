using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Moives.Api.Auth;
using Moives.Api.Mapping;
using Movies.Api.Auth;
using Movies.Application.Service;
using Movies.Contracts.Requsets;


namespace Movies.Api.Controllers;

[ApiController]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IOutputCacheStore _outputCache;
    public MovieController(IMovieService movieService, IOutputCacheStore outputCache)
    {
        _movieService = movieService;
        _outputCache = outputCache;
    }

    [Authorize(Policy = AuthConstants.AdminUserPolicyName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, token);

        var response = movie.MapToMovieResponse();
        await _outputCache.EvictByTagAsync("movies", token);

        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
    }


    [Authorize(Policy = AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie(id);
        var userid = HttpContext.GetUserId();
        var result = await _movieService.UpdateAsync(movie, userid, token);
        if (result is null)
        {
            return NotFound();
        }
        var response = movie.MapToMovieResponse();
        await _outputCache.EvictByTagAsync("movies", token);

        return Ok(response);
    }

    [Authorize(Policy = AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var userid = HttpContext.GetUserId();
        var result = await _movieService.DeleteByIdAsync(id, userid!.Value, token);
        if (!result)
        {
            return NotFound();
        }
        await _outputCache.EvictByTagAsync("movies", token);
        return Ok();
    }

    [OutputCache(PolicyName = "MovieCache")]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, token);

        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.MapToMovieResponse();
        return Ok(response);
    }

    [OutputCache(PolicyName = "MovieCache")]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMovieRequest request, CancellationToken token)
    {
        var userid = HttpContext.GetUserId();
        var options = request.MapToOptions()
            .WithUser(userid);
        var count = await _movieService.GetCountAsync(request.Title, request.YearOfRelease, token);
        var movies = await _movieService.GetAllAsync(options, token);
        var response = movies.MapToMoviesResponse(request.Page, request.PageSize, count);

        return Ok(response);
    }

}
