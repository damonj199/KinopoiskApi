using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Application;

public interface ISyncService
{
    Task<IReadOnlyList<Movie>> SyncMovieDataAsync(string jsonResponse, CancellationToken cancellationToken);
}
