using System.Globalization;

using AutoMapper;

using Cronos;

using KinopoiskDB.Core.Models;
using KinopoiskDB.Dal.PostgreSQL;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KinopoiskDB.Infrastructure;

public class MoviesSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly KinopoiskService _kinopoiskService;
    private readonly ILogger<MoviesSyncService> _logger;
    private readonly CronExpression _cronExpression;
    private readonly IMapper _mapper;
    private TimeZoneInfo _timeZone;

    public MoviesSyncService(IServiceProvider serviceProvider, KinopoiskService kinopoiskService, ILogger<MoviesSyncService> logger, IMapper mapper)
    {
        _serviceProvider = serviceProvider;
        _kinopoiskService = kinopoiskService;
        _logger = logger;
        _mapper = mapper;

        _cronExpression = CronExpression.Parse("0 0 1 * *");
        _timeZone = TimeZoneInfo.Local;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var next = _cronExpression.GetNextOccurrence(DateTime.UtcNow, _timeZone);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds > 0)
                {
                    await Task.Delay(delay, stoppingToken);
                }

                await SyncMoviesAsync(stoppingToken);
            }
        }
    }

    private async Task SyncMoviesAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Ыtart synchronization");

        var currentYear = DateTime.UtcNow.Year;
        var currentMonth = DateTime.UtcNow.Month;
        var monthString = currentMonth.ToString("MMMM", CultureInfo.InvariantCulture);

        _logger.LogInformation($"Получили год {currentYear} и месяц {currentMonth} - {monthString} для обновления");

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var movies = await _kinopoiskService.SyncPremieresBackgrondAsync(currentYear, monthString, stoppingToken);

                var dbContext = scope.ServiceProvider.GetRequiredService<KinopoiskDbContext>();

                foreach (var movieDto in movies)
                {
                    _logger.LogInformation("проверяем есть ли такой фильм в БД, если есть отменяем добавление");
                    var exisitgMovie = await dbContext.Movies.FirstOrDefaultAsync(m => m.KinopoiskId == movieDto.KinopoiskId, stoppingToken);

                    _logger.LogInformation("Если фильма нет, добавляем его в БД");
                    if (exisitgMovie == null)
                    {
                        var movie = _mapper.Map<Movie>(movieDto);

                        dbContext.Movies.Add(movie);
                    }
                    else
                    {
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
}
