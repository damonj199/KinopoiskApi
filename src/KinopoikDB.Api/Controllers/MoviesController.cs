using System.Net;

using KinopoiskDB.Application;
using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Application.Models;

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
    /// <summary>
    /// Поиск Фильмов по названию
    /// </summary>
    /// <remarks>В этом эндпоинте можно получить списов фильмов по названию</remarks>
    /// <param name="title"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Успешное выполнение</response>
    [HttpGet("search-by-name")]
    [ProducesResponseType(typeof(List<MovieDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SearchMoviesByNameAsync([FromQuery] string title, CancellationToken cancellationToken)
    {
        var movies = await _kinopoiskService.SearchMoviesByNameAsync(title, cancellationToken);
        return Ok(movies);
    }
    /// <summary>
    /// Фильтрация фильмов по жарнам и странам
    /// </summary>
    /// <remarks>Данный эндпоинт отфильтрует фильмы по выбранным жанрам и странам</remarks>
    /// <param name="movieRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("search-by-filter")]
    [ProducesResponseType(typeof(List<MovieDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetMoviesByFilterAsync([FromQuery]MovieRequest movieRequest, CancellationToken cancellationToken)
    {
        var movies = await _kinopoiskService.GetMoviesByFilterAsync(movieRequest, cancellationToken);
        return Ok(movies);
    }
    /// <summary>
    /// Поиск премьер за определенный месяц
    /// </summary>
    /// <remarks>Здесь можно получить фильмов вышедших в выбраный месяц и год</remarks>
    /// <param name="premiereRequest">Указать год и месяц</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Список премьер за выбранный месяц выбранного года</returns>
    [HttpGet("premieres")]
    [ProducesResponseType(typeof(List<MovieDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPremieresForMonthAsync([FromQuery] PremiereRequest premiereRequest, CancellationToken cancellationToken)
    {
        var premieres = await _kinopoiskService.GetPremieresForMonthAsync(premiereRequest, cancellationToken);
        return Ok(premieres);
    }
    /// <summary>
    /// Тестовая ручка на добавление фильмов
    /// </summary>
    /// <param name="premiereRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>количество добавленных фильмов в БД</returns>
    [HttpPost("films")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddFilmsAsync([FromQuery] PremiereRequest premiereRequest, CancellationToken cancellationToken)
    {
        var films = await _kinopoiskService.AddFilms(premiereRequest, cancellationToken);
        return Ok(films);
    }

    /// <summary>
    /// Здесь можно посмотреть все фильм которые есть постранично!
    /// </summary>
    ///  <remarks>Нужно указать Page - номер страницы и PageSize - количество фильмов на странице</remarks>
    /// <param name="pagedRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Пагинированный список</returns>
    [HttpGet("all-films")]
    [ProducesResponseType(typeof(List<MovieDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllMoviesAsync([FromQuery] PagedRequest<MovieDto> pagedRequest, [FromQuery] SortRequest<MovieDto> sortRequest, CancellationToken cancellationToken)
    {
        var premieres = await _kinopoiskService.GetAllMoviesAsync(pagedRequest, sortRequest, cancellationToken);
        return Ok(premieres);
    }
}