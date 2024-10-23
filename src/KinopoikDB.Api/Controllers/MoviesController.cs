using System.Net;

using KinopoiskDB.Application;
using KinopoiskDB.Application.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace KinopoikDB.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class MoviesController : ControllerBase
{
    private readonly IKinopoiskService _kinopoiskService;

    public MoviesController(IKinopoiskService kinopoiskService)
    {
        _kinopoiskService = kinopoiskService;
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<MovieDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SearchMoviesAsync([FromQuery] string title, [FromQuery] int? year, CancellationToken cancellationToken)
    {
        var movies = await _kinopoiskService.SearchMoviesAsync(default!, default, cancellationToken);
        return Ok(movies);
    }

    [HttpGet("premieres")]
    public async Task<ActionResult<List<MovieDto>>> GetPremieresAsync([FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        var premieres = await _kinopoiskService.GetPremieresAsync(year, month, cancellationToken);
        return Ok(premieres);
    }
}
