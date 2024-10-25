using KinopoiskDB.Application;
using KinopoiskDB.Core.Enum;
using KinopoiskDB.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace KinopoiskDB.Dal.PostgreSQL.Repository;

public class MoviesRepository : BaseRepository, IMoviesRepository
{
    public MoviesRepository(KinopoiskDbContext connectionString) : base(connectionString) { }

    public async Task<List<Movie>> AddMoviesAsync(List<Movie> movies)
    {
        await _cxt.Movies.AddRangeAsync(movies);
        await _cxt.SaveChangesAsync();

        return movies;
    }

    public async Task<List<Movie>> GetPremieresAsync(int year, Month month, CancellationToken cancellationToken)
    {
        var primieres = await _cxt.Movies
            .AsNoTracking()
            .Where(m => m.Year == year)
            .Select(m => new Movie
            {
                KinopoiskId = m.KinopoiskId,
                NameRu = m.NameRu,
                NameEn = m.NameEn,
                NameOriginal = m.NameOriginal,
                Year = m.Year,
                Month = m.Month,
                PosterUrl = m.PosterUrl,
                Description = m.Description,
                Countries = m.Countries,
                Genres = m.Genres,
            }).ToListAsync();

        return primieres;
    }
}
