using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Application;

public interface IMoviesRepository
{
    Task<List<Movie>> SyncMovieDataAsync(List<Movie> movies);
}
