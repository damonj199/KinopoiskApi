using KinopoikDB.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KinopoikDB.Api.Controllers;

public class MoviesController : Controller
{
    private readonly IKinopoiskService _kinopoiskService;

    public MoviesController(IKinopoiskService kinopoiskService)
    {
        _kinopoiskService = kinopoiskService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string title, [FromQuery] int? year, [FromQuery] string genre)
    {
        var movies = await _kinopoiskService.SearchMoviesAsync(title, year, genre);
        return Ok(movies);
    }

    [HttpGet("premieres")]
    public async Task<IActionResult> GetPremieres()
    {
        var currentDate = DateTime.Now;
        var premieres = await _kinopoiskService.GetPremieresAsync(currentDate);
        return Ok(premieres);
    }
}
