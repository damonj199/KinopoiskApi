using Cronos;

using KinopoiskDB.Application;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KinopoiskDB.Infrastructure;

public sealed partial class MoviesSyncService : BackgroundService
{
    private readonly ISyncService _syncService;
    private readonly ILogger<MoviesSyncService> _logger;
    private readonly CronExpression _cronExpression;
    private readonly TimeZoneInfo _timeZone;

    public MoviesSyncService(ISyncService syncService, ILogger<MoviesSyncService> logger)
    {
        _syncService = syncService;
        _logger = logger;

        _cronExpression = CronExpression.Parse("0 0 1 * *");
        _timeZone = TimeZoneInfo.Local;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var next = _cronExpression.GetNextOccurrence(DateTime.UtcNow, _timeZone);
                if (next.HasValue)
                {
                    var delay = next.Value - DateTimeOffset.Now;
                    if (delay.TotalMilliseconds > 0)
                    {
                        await Task.Delay(delay, stoppingToken);
                    }

                    await _syncService.SyncMoviesAsync(stoppingToken);
                }

            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в выполнении синхронизации фильмов");
            }
        }
        _logger.LogInformation("Background service завершает работу.");
    }
}
