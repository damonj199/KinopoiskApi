using KinopoiskDB.Application;
using KinopoiskDB.Application.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace KinopoikDB.Api.Controllers;
[ApiController]
[Route("api/movie/")]
public class MoviesController : Controller
{
    private readonly IKinopoiskService _kinopoiskService;

    public MoviesController(IKinopoiskService kinopoiskService)
    {
        _kinopoiskService = kinopoiskService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<MoviesDto>>> SearchMoviesAsync([FromQuery] string title, [FromQuery] int? year, CancellationToken cancellationToken)
    {
        var movies = await _kinopoiskService.SearchMoviesAsync(title, year, cancellationToken);
        return Ok(movies);
    }

    [HttpGet("premieres")]
    public async Task<ActionResult<List<MoviesDto>>> GetPremieresAsync([FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        var premieres = await _kinopoiskService.GetPremieresAsync(year, month, cancellationToken);
        return Ok(premieres);
    }
}
