using System.Text.Json;

using AutoMapper;

using KinopoiskDB.Application;
using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Core.Models;

using Microsoft.Extensions.Configuration;

namespace KinopoiskDB.Infrastructure;

public class KinopoiskService : IKinopoiskService
{
    private readonly IMoviesRepository _moviesRepository;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly string _apiKey;
    private readonly string _apiUrl;

    public KinopoiskService(HttpClient httpClient, IConfiguration configuration, IMoviesRepository moviesRepository, IMapper mapper)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _apiUrl = configuration["Kinopoisk:ApiUrl"];
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", configuration["Kinopoisk:ApiKey"]);
        _moviesRepository = moviesRepository;
    }

    public async Task<IReadOnlyList<MovieDto>> SearchMoviesAsync(string title, int? year, CancellationToken cancellationToken)
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

        return new List<MovieDto>();
    }

    public async Task<IReadOnlyList<MovieDto>> GetPremieresAsync(int year, int month, CancellationToken cancellationToken)
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

    public async Task<IReadOnlyList<MovieDto>> SyncMovieDataAsync(string jsonResponse)
    {
        var moviesPageDto = JsonSerializer.Deserialize<MoviesPageDto>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        List<MovieDto> moviesDto = moviesPageDto?.Items ?? new List<MovieDto>();

        List<Movie> movies = MapMoviesDtoToMovies(moviesDto);

        var films = await _moviesRepository.SyncMovieDataAsync(movies);

        return _mapper.Map<List<MovieDto>>(films);
    }

    public List<Movie> MapMoviesDtoToMovies(List<MovieDto> movieDtos)
    {
        return movieDtos.Select(dto => new Movie
        {
            Id = dto.Id,
            KinopoiskId = dto.KinopoiskId,
            NameRu = dto.NameRu,
            NameEn = dto.NameEn,
            NameOriginal = dto.NameOriginal,
            Year = dto.Year,
            PosterUrl = dto.PosterUrl,
            Description = dto.Description,
            Countries = dto.Countries
                .Select(c => new Country
                {
                    Value = c.Country,
                })
                .ToList(),
            Genres = dto.Genres
                .Select(g => new Genre
                {
                    Value = g.Genre   
                })
                .ToList()
        }).ToList();
    }
}
