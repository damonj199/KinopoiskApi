using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Dal.PostgreSQL.Repository;

public class MoviesRepository : BaseRepository, IMoviesRepository
{
    public MoviesRepository(KinopoiskDbContext connectionString) : base(connectionString) { }

    public async Task<List<Movies>> SyncMovieDataAsync(List<Movies> movies)
    {
        await _cxt.Movies.AddRangeAsync(movies);
        await _cxt.SaveChangesAsync();

        return movies;
    }
}
