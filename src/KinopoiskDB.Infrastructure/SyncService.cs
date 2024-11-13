using System.Text.Json;

using AutoMapper;

using KinopoiskDB.Application;
using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Core.Models;

using Microsoft.Extensions.Logging;

namespace KinopoiskDB.Infrastructure;

public class SyncService : ISyncService
{
    private readonly IMapper _mapper;
    private readonly ILogger<SyncService> _logger;

    public SyncService(ILogger<SyncService> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Movie>> SyncMovieDataAsync(string jsonResponse, CancellationToken cancellationToken)
    {
        var moviesPageDto = JsonSerializer.Deserialize<MoviesPageDto>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var moviesDto = moviesPageDto?.Items ?? new List<MovieDto>();

        var movies = _mapper.Map<IReadOnlyList<Movie>>(moviesDto);

        return movies;
    }
}
