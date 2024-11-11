using System.Globalization;

using AutoMapper;

using Cronos;

using KinopoiskDB.Application;
using KinopoiskDB.Core.Enum;
using KinopoiskDB.Dal.PostgreSQL;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KinopoiskDB.Infrastructure;

public class MoviesSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MoviesSyncService> _logger;
    private readonly CronExpression _cronExpression;
    private readonly IMapper _mapper;
    private TimeZoneInfo _timeZone;

    public MoviesSyncService(IServiceProvider serviceProvider, ILogger<MoviesSyncService> logger, IMapper mapper)
    {
        _serviceProvider = serviceProvider;
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
                //await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                await SyncMoviesAsync(stoppingToken);
            }
        }
    }

    private async Task SyncMoviesAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start synchronization");

        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        var currentYear = DateTime.Now.Year;
        var currentMonth = (Month)DateTime.Now.Month;

        _logger.LogInformation($"Получили год {currentYear}, и месяц {currentMonth}, для обновления");

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<KinopoiskDbContext>();
                var kinopoiskService = scope.ServiceProvider.GetRequiredService<IKinopoiskService>();

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
}
