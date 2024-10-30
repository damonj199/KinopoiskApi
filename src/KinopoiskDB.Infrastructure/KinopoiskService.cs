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

    public async Task<IReadOnlyList<MovieDto>> GetMoviesByFilterAsync(MovieRequest movieRequest, CancellationToken cancellationToken)
    {
        var movies = await _moviesRepository.GetMoviesByFilterAsync(movieRequest, cancellationToken);
        var result = _mapper.Map<List<MovieDto>>(movies);

        return result;
    }

    public async Task<int> AddFilms(PremiereRequest premiereRequest, CancellationToken cancellationToken)
    {
        var year = premiereRequest.Year;
        var month = premiereRequest.Month;

        var request = string.Format(_settings.PremieresEndpoint, year, month, cancellationToken);
        var response = await _httpClient.GetAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = await SyncMovieDataAsync(jsonResponse, cancellationToken);
        var kinopoiskId = result.Count();

        return kinopoiskId;
    }

    public async Task<IReadOnlyList<MovieDto>> SearchMoviesByNameAsync(string title, CancellationToken cancellationToken)
    {
        
        var moviesByName = await _moviesRepository.SearchMoviesByNameAsync(title, cancellationToken);
        var result = _mapper.Map<IReadOnlyList<MovieDto>>(moviesByName);

        if(result.Count == 0)
        {
            var request = string.Format(_settings.FilmsEndpoint, title, cancellationToken);
            var response = await _httpClient.GetAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            var res = await SyncMovieDataAsync(jsonResponse, cancellationToken);
            result = res;
        }

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

    public async Task<IReadOnlyList<MovieDto>> SyncMovieDataAsync(string jsonResponse, CancellationToken cancellationToken)
    {
        var moviesPageDto = JsonSerializer.Deserialize<MoviesPageDto>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var moviesDto = moviesPageDto?.Items ?? new List<MovieDto>();

        var movies = _mapper.Map<List<Movie>>(moviesDto);

        var films = await _moviesRepository.AddMoviesAsync(movies, cancellationToken);

        return _mapper.Map<List<MovieDto>>(films);
    }
}
