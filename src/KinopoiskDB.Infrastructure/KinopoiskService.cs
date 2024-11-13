using System.Text.Json;

using AutoMapper;

using KinopoiskDB.Application;
using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Application.Models;
using KinopoiskDB.Core.Models;
using KinopoiskDB.Infrastructure.Settings;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace KinopoiskDB.Infrastructure;

public class KinopoiskService : IKinopoiskService
{
    private readonly IMoviesRepository _moviesRepository;
    private readonly ISyncService _syncService;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly KinopoiskSettings _settings;
    private readonly IDistributedCache _cache;

    public KinopoiskService(HttpClient httpClient, IMoviesRepository moviesRepository, IMapper mapper,
        IDistributedCache distributedCache, IOptions<KinopoiskSettings> settings, ISyncService syncService)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _moviesRepository = moviesRepository;
        _syncService = syncService;
        _settings = settings.Value;
        _cache = distributedCache;
    }

    public async Task<IReadOnlyList<MovieDto>> GetPremieresForMonthAsync(PremiereRequest premiereRequest, CancellationToken cancellationToken)
    {
        var cacheKey = $"premieres_{premiereRequest.Year}.{(int)premiereRequest.Month}";

        var premiereRuStart = new DateOnly(premiereRequest.Year, (int)premiereRequest.Month, 1);
        var premiereRuEnd = premiereRuStart.AddMonths(1).AddDays(-1);

        var cachedMovies = await _cache.GetStringAsync(cacheKey);

        if (cachedMovies != null && cachedMovies.Length > 2)
        {
            return JsonSerializer.Deserialize<IReadOnlyList<MovieDto>>(cachedMovies);
        }

        var movies = await _moviesRepository.GetPremieresForMonthAsync(premiereRuStart, premiereRuEnd, cancellationToken);
        var premiere = _mapper.Map<IReadOnlyList<MovieDto>>(movies);

        await SaveCachedAsync(premiere, cacheKey, cancellationToken);

        return premiere ?? [];
    }

    public async Task<IReadOnlyList<MovieDto>> GetMoviesByFilterAsync(MovieRequest movieRequest, CancellationToken cancellationToken)
    {
        var genres = String.Join(",", movieRequest.Genres);
        var countries = String.Join(",", movieRequest.Countries);

        var cacheKey = $"byFilter_{genres}.{countries}";

        var cachedMovies = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedMovies != null && cachedMovies.Length > 2)
        {
            return JsonSerializer.Deserialize<IReadOnlyList<MovieDto>>(cachedMovies);
        }

        var movies = await _moviesRepository.GetMoviesByFilterAsync(genres, countries, cancellationToken);
        var result = _mapper.Map<IReadOnlyList<MovieDto>>(movies);

        await SaveCachedAsync(result, cacheKey, cancellationToken);

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
        var listMovies = await _syncService.SyncMovieDataAsync(jsonResponse, cancellationToken);
        var result = await _moviesRepository.AddMoviesAsync(listMovies, cancellationToken);

        var kinopoiskId = result.Count();
        return kinopoiskId;
    }

    public async Task<IReadOnlyList<MovieDto>> SearchMoviesByNameAsync(string title, CancellationToken cancellationToken)
    {
        var cacheKey = $"Movies_{title}";

        var cachedMovies = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedMovies != null && cachedMovies.Length > 2)
        {
            return JsonSerializer.Deserialize<IReadOnlyList<MovieDto>>(cachedMovies);
        }

        var moviesByName = await _moviesRepository.SearchMoviesByNameAsync(title, cancellationToken);
        var result = _mapper.Map<IReadOnlyList<MovieDto>>(moviesByName);

        if (result.Count == 0)
        {
            var res = await SearchMoviesByNameKinopoisApi(title, cancellationToken);
            result = res;
        }

        await SaveCachedAsync(result, cacheKey, cancellationToken);

        return result ?? [];
    }

    public async Task<IReadOnlyList<MovieDto>> SearchMoviesByNameKinopoisApi(string title, CancellationToken cancellationToken)
    {
        var request = string.Format(_settings.FilmsEndpoint, title, cancellationToken);
        var response = await _httpClient.GetAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        var listMovies = await _syncService.SyncMovieDataAsync(jsonResponse, cancellationToken);
        var result = await _moviesRepository.AddMoviesAsync(listMovies, cancellationToken);

        return _mapper.Map<IReadOnlyList<MovieDto>>(result) ?? [];
    }

    public async Task<IReadOnlyList<Movie>> SyncPremieresBackgrondAsync(int year, string? month, CancellationToken cancellationToken)
    {
        var request = string.Format(_settings.PremieresEndpoint, year, month);
        var response = await _httpClient.GetAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var listMovies = await _syncService.SyncMovieDataAsync(jsonResponse, cancellationToken);

        return listMovies;
    }

    public async Task SaveCachedAsync(IReadOnlyList<MovieDto> cacheMovies, string cacheKey, CancellationToken cancellationToken)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cacheMovies), cacheOptions, cancellationToken);
    }

    public async Task Delete()
    {
        await _moviesRepository.RemovePreviousMonthPremieresAsync();
    }

    public async Task<IReadOnlyList<MovieDto>> GetAllMoviesAsync(PagedResponse<MovieDto> pageRes, CancellationToken cancellationToken)
    {        
        var result = await _moviesRepository.GetAllMoviesAsync(pageRes.Page, pageRes.PageSize, cancellationToken);

        return _mapper.Map<IReadOnlyList<MovieDto>>(result) ?? [];
    }
}
