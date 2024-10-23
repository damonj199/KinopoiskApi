using System.Text.Json;

using AutoMapper;

using KinopoiskDB.Application;
using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Core.Models;
using KinopoiskDB.Dal.PostgreSQL.Repository;

using Microsoft.Extensions.Configuration;

namespace KinopoiskDB.Infrastructure;

public class KinopoiskService : IKinopoiskService
{
    private readonly IMoviesRepository _moviesRepository;
    private readonly IKinopoiskService _kinopoiskService;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly string _apiKey;
    private readonly string _apiUrl;

    public KinopoiskService(HttpClient httpClient, IKinopoiskService kinopoiskService, IConfiguration configuration, IMoviesRepository moviesRepository, IMapper mapper)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _apiUrl = configuration["Kinopoisk:ApiUrl"];
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", configuration["Kinopoisk:ApiKey"]);
        _kinopoiskService = kinopoiskService;
        _moviesRepository = moviesRepository;
    }

    public async Task<List<MoviesDto>> SearchMoviesAsync(string title, int? year, CancellationToken cancellationToken)
    {
        var hasTitle = !string.IsNullOrEmpty(title);
        var hasYear = year > 1000;
        const string url = "v2.2/films";
        var titlePart = $"?keyword={title}";
        var yearPart = $"&yearFrom={year}&yearTo={year}";

        using var cts = new CancellationTokenSource();

        if (hasTitle && hasYear)
        {
            var requestTitleAndYear = _apiUrl + url + titlePart + yearPart;

            var responseTitleAndYaer = await _httpClient.GetAsync(requestTitleAndYear, cts.Token);
            var jsonResponse = await responseTitleAndYaer.Content.ReadAsStringAsync(cts.Token);

            var result = await SyncMovieDataAsync(jsonResponse);

            return result;
        }

        if (hasTitle)
        {
            var requestForTitle = _apiUrl + url + titlePart;
            var responseForTitle = await _httpClient.GetAsync(requestForTitle, cts.Token);
            var jsonResponse = await responseForTitle.Content.ReadAsStringAsync(cts.Token);

            var result = await SyncMovieDataAsync(jsonResponse);

            return result;
        }

        return new List<MoviesDto>();
    }

    public async Task<List<MoviesDto>> GetPremieresAsync(int year, int month, CancellationToken cancellationToken)
    {
        const string url = "v2.2/films/premieres";
        var currentDate = DateTimeOffset.UtcNow.Date;
        var yearAndMonthPart = $"?year={year}&month={GetMonthByNumber(currentDate.Month)}";
        var request = _apiUrl + url + yearAndMonthPart;

        var cts = new CancellationTokenSource();

        var response = await _httpClient.GetAsync(request, cts.Token);
        var jsonResponse = await response.Content.ReadAsStringAsync(cts.Token);

        var result = await SyncMovieDataAsync(jsonResponse);

        return result;
    }

    private static string GetMonthByNumber(int month)
    {
        return month switch
        {
            1 => "JANUARY",
            2 => "FEBRUARY",
            3 => "MARCH",
            4 => "APRIL",
            5 => "MAY",
            6 => "JUNE",
            7 => "JULY",
            8 => "AUGUST",
            9 => "SEPTEMBER",
            10 => "OCTOBER",
            11 => "NOVEMBER",
            12 => "DECEMBER",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<List<MoviesDto>> SyncMovieDataAsync(string jsonResponse)
    {
        var moviesPageDto = JsonSerializer.Deserialize<MoviesPageDto>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        List<MoviesDto> moviesDto = moviesPageDto?.Items ?? new List<MoviesDto>();

        List<Movies> movies = MapMoviesDtoToMovies(moviesDto);

        var films = await _moviesRepository.SyncMovieDataAsync(movies);

        return _mapper.Map<List<MoviesDto>>(films);
    }

    public List<Movies> MapMoviesDtoToMovies(List<MoviesDto> movieDtos)
    {
        return movieDtos.Select(dto => new Movies
        {
            Id = dto.Id,
            KinopoiskId = dto.KinopoiskId,
            NameRu = dto.NameRu,
            NameEn = dto.NameEn,
            NameOriginal = dto.NameOriginal,
            Year = dto.Year,
            PosterUrl = dto.PosterUrl,
            Description = dto.Description,
            Countries = dto.Countries,
            Genres = dto.Genres
        }).ToList();
    }
}
