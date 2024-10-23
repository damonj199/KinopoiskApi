using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Dal.PostgreSQL.Repository;

public interface IMoviesRepository
{
    Task<List<Movies>> SyncMovieDataAsync(List<Movies> movies);
}
