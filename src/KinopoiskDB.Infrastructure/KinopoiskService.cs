using System.Net.Http.Json;
using System.Text.Json;

using AutoMapper;

using KinopoiskDB.Application;
using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Core.Models;
using KinopoiskDB.Infrastructure.Settings;

using Microsoft.Extensions.Options;

namespace KinopoiskDB.Infrastructure;

public class KinopoiskService : IKinopoiskService
{
    private readonly IMoviesRepository _moviesRepository;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly KinopoiskSettings _settings;

    public KinopoiskService(HttpClient httpClient, IMoviesRepository moviesRepository, IMapper mapper, IOptions<KinopoiskSettings> settings)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _moviesRepository = moviesRepository;
        _settings = settings.Value;
    }

    public async Task<IReadOnlyList<MovieDto>> GetPremieresAsync(PremiereRequest premiereRequest, CancellationToken cancellationToken)
    {
        var year = premiereRequest.Year;
        var month = premiereRequest.Month;

        var movies = await _moviesRepository.GetPremieresAsync(year, month, cancellationToken);
        var premiere = _mapper.Map<List<MovieDto>>(movies);

        return premiere ?? [];
    }

    public async Task<int> AddFilms(PremiereRequest premiereRequest, CancellationToken cancellationToken)
    {
        var year = premiereRequest.Year;
        var month = premiereRequest.Month;

        var request = string.Format(_settings.PremieresEndpoint, year, month, cancellationToken);
        var response = await _httpClient.GetAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = await SyncMovieDataAsync(jsonResponse);
        var kinopoiskId = result.Count();

        return kinopoiskId;
    }

    public async Task<IReadOnlyList<MovieDto>> SearchMoviesAsync(string title, int? year, CancellationToken cancellationToken)
    {
        var request = string.Format(_settings.FilmsEndpoint, title, year);
        var response = await _httpClient.GetAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();
        //var jsonResponse = await response.Content.ReadFromJsonAsync<MovieDto[]>(cancellationToken);

        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = await SyncMovieDataAsync(jsonResponse);

        return result ?? [];
    }

    public async Task<IReadOnlyList<MovieDto>> SyncPremieresBackgrondAsync(int year, string? month, CancellationToken cancellationToken)
    {
        var request = string.Format(_settings.PremieresEndpoint, year, month);
        var response = await _httpClient.GetAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadFromJsonAsync<MovieDto[]>(cancellationToken);

        return jsonResponse ?? [];
    }

    public async Task<IReadOnlyList<MovieDto>> SyncMovieDataAsync(string jsonResponse)
    {
        var moviesPageDto = JsonSerializer.Deserialize<MoviesPageDto>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        List<MovieDto> moviesDto = moviesPageDto?.Items ?? new List<MovieDto>();

        List<Movie> movies = _mapper.Map<List<Movie>>(moviesDto);

        var films = await _moviesRepository.AddMoviesAsync(movies);

        return _mapper.Map<List<MovieDto>>(films);
    }
}
