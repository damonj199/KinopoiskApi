using System.Text.Json;

using AutoMapper;

using KinopoiskDB.Application;
using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Core.Enum;
using KinopoiskDB.Core.Models;
using KinopoiskDB.Dal.PostgreSQL;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KinopoiskDB.Infrastructure;

public class SyncService : ISyncService
{
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SyncService> _logger;

    public SyncService(ILogger<SyncService> logger, IMapper mapper, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
    }

    public IReadOnlyList<Movie> SyncMovieData(string jsonResponse, CancellationToken cancellationToken)
    {
        var moviesPageDto = JsonSerializer.Deserialize<MoviesPageDto>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var moviesDto = moviesPageDto?.Items ?? new List<MovieDto>();

        var movies = _mapper.Map<IReadOnlyList<Movie>>(moviesDto);

        return movies;
    }
    public async Task SyncMoviesAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Начало синхронизации");

        var currentYear = DateTime.Now.Year;
        var currentMonth = (Month)DateTime.Now.Month;

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<KinopoiskDbContext>();
                var kinopoiskService = scope.ServiceProvider.GetRequiredService<IKinopoiskService>();

                await RemovePreviousMonthPremieresAsync(dbContext, stoppingToken);

                var movies = await kinopoiskService.SyncPremieresBackgrondAsync(currentYear, currentMonth.ToString(), stoppingToken);

                foreach (var movieDto in movies)
                {
                    _logger.LogInformation("проверяем есть ли такой фильм в БД, если есть отменяем добавление");
                    var exisitgMovie = await dbContext.Movies.FirstOrDefaultAsync(m => m.KinopoiskId == movieDto.KinopoiskId, stoppingToken);

                    if (exisitgMovie == null)
                    {
                        _logger.LogInformation("Фильма в БД нет, добавляем его");
                        dbContext.Movies.Add(movieDto);
                    }
                    else
                    {
                        _logger.LogInformation("Фильм уже у нас есть, обновляем данные по нему");
                        dbContext.Movies.Update(exisitgMovie);
                    }
                }
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            _logger.LogInformation("Кинопремьеры обнолены!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Синхронизация завершилась ошибкой");
        }
    }

    private async Task RemovePreviousMonthPremieresAsync(KinopoiskDbContext dbContext, CancellationToken stoppingToken)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        var startOfCurrentMonth = new DateOnly(now.Year, now.Month, 1);
        var startOfPreviousMonth = startOfCurrentMonth.AddMonths(-1);
        var endOfPreviousMonth = startOfCurrentMonth.AddDays(-1);

        var oldPremieres = await dbContext.Movies
            .AsNoTracking()
            .Where(p => p.PremiereRu >= startOfPreviousMonth && p.PremiereRu <= endOfPreviousMonth)
            .Select(m => new Movie
            {
                Id = m.Id
            })
            .ToListAsync(stoppingToken);

        dbContext.Movies.RemoveRange(oldPremieres);
        await dbContext.SaveChangesAsync(stoppingToken);
    }
}
