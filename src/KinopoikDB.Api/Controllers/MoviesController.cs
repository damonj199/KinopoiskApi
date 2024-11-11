using System.Net;

using KinopoiskDB.Application;
using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Core.Models;

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

    [HttpGet("search-by-name")]
    [ProducesResponseType(typeof(List<MovieDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SearchMoviesByNameAsync([FromQuery] string title, CancellationToken cancellationToken)
    {
        var movies = await _kinopoiskService.SearchMoviesByNameAsync(title, cancellationToken);
        return Ok(movies);
    }

    [HttpGet("search-by-filter")]
    [ProducesResponseType(typeof(List<MovieDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetMoviesByFilterAsync([FromQuery]MovieRequest movieRequest, CancellationToken cancellationToken)
    {
        var movies = await _kinopoiskService.GetMoviesByFilterAsync(movieRequest, cancellationToken);
        return Ok(movies);
    }

    [HttpGet("premieres")]
    [ProducesResponseType(typeof(List<MovieDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPremieresForMonthAsync([FromQuery] PremiereRequest premiereRequest, CancellationToken cancellationToken)
    {
        var premieres = await _kinopoiskService.GetPremieresForMonthAsync(premiereRequest, cancellationToken);
        return Ok(premieres);
    }

    [HttpPost("films")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddFilmsAsync([FromQuery] PremiereRequest premiereRequest, CancellationToken cancellationToken)
    {
        var films = await _kinopoiskService.AddFilms(premiereRequest, cancellationToken);
        return Ok(films);
    }
}
