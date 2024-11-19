using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Application;

public interface ISyncService
{
    IReadOnlyList<Movie> SyncMovieData(string jsonResponse, CancellationToken cancellationToken);
    Task SyncMoviesAsync(CancellationToken stoppingToken);
}
