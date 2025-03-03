using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moives.Api.Mapping;
using Movies.Api.Auth;
using Movies.Application.Service;
using Movies.Contracts.Requsets;

namespace Movies.Api.Controllers;

[ApiController]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovieAsync([FromRoute] Guid id,
      [FromBody] RatingRequset requset, CancellationToken token = default)
    {
        var userId = HttpContext.GetUserId();

        return await _ratingService.RateMovieAsync(id, userId!.Value, requset.rating, token) ?
            Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    public async Task<IActionResult> DeleteRatingAsync([FromRoute] Guid id, CancellationToken token = default)
    {
        var userId = HttpContext.GetUserId();
        return await _ratingService.DeleteRatingAsync(id, userId!.Value, token) ?
            Ok() : NotFound();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRating)]
    public async Task<IActionResult> GetUserRatingAsync( CancellationToken token = default)
    {
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.GetUserRatingsAsync(userId!.Value, token);
        var response = result.MapToResponse();
        return Ok(response);
    }

}
