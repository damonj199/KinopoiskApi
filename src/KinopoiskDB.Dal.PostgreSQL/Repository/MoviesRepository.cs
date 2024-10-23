using KinopoiskDB.Application;
using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Dal.PostgreSQL.Repository;

public class MoviesRepository : BaseRepository, IMoviesRepository
{
    public MoviesRepository(KinopoiskDbContext connectionString) : base(connectionString) { }

    public async Task<List<Movie>> SyncMovieDataAsync(List<Movie> movies)
    {
        await _cxt.Movies.AddRangeAsync(movies);
        await _cxt.SaveChangesAsync();

        return movies;
    }
}
