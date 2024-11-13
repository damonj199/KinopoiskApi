using KinopoiskDB.Application;
using KinopoiskDB.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace KinopoiskDB.Dal.PostgreSQL.Repository;

public class MoviesRepository : BaseRepository, IMoviesRepository
{
    public MoviesRepository(KinopoiskDbContext connectionString) : base(connectionString) { }

    public async Task<IReadOnlyList<Movie>> AddMoviesAsync(IReadOnlyList<Movie> movies, CancellationToken cancellationToken)
    {
        foreach (var movie in movies)
        {
            var exisitgMovie = await _cxt.Movies
                .FirstOrDefaultAsync(m => m.KinopoiskId == movie.KinopoiskId, cancellationToken);

            _ = (exisitgMovie == null) ? _cxt.Movies.Add(movie) : _cxt.Movies.Update(exisitgMovie);
        }
        await _cxt.SaveChangesAsync(cancellationToken);

        return movies;
    }

    public async Task<IReadOnlyList<Movie>> GetMoviesByFilterAsync(string genres, string countries, CancellationToken cancellationToken)
    {
        var query = _cxt.Movies.Include(g => g.Genres).Include(c => c.Countries).AsQueryable();

        if (!string.IsNullOrWhiteSpace(genres))
            query = query.Where(m => m.Genres.Any() && m.Genres.Any(g => genres.Contains(g.Value)));

        if (!string.IsNullOrWhiteSpace(countries))
            query = query.Where(m => m.Countries.Any() && m.Countries.Any(c => countries.Contains(c.Value)));

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<Movie>> GetPremieresForMonthAsync(DateOnly premiereRuStart, DateOnly premiereRuEnd, CancellationToken cancellationToken)
    {
        var primieres = await _cxt.Movies
            .AsNoTracking()
            .Where(m => m.PremiereRu >= premiereRuStart && m.PremiereRu <= premiereRuEnd)
            .Include(g => g.Genres)
            .Include(c => c.Countries)
            .ToListAsync(cancellationToken);

        return primieres;
    }

    public async Task<IReadOnlyList<Movie>> SearchMoviesByNameAsync(string title, CancellationToken cancellationToken)
    {
        var moviesByName = await _cxt.Movies
            .AsNoTracking()
            .Where(m => m.NameRu.ToLower().Contains(title.ToLower()) || m.NameEn.ToLower().Contains(title.ToLower()))
            .Include(g => g.Genres)
            .Include(c => c.Countries)
            .ToListAsync(cancellationToken);

        return moviesByName;
    }

    public async Task<IReadOnlyList<Movie>> GetAllMoviesAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var allMovies = await _cxt.Movies
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(g => g.Genres)
            .Include(c => c.Countries)
            .ToListAsync(cancellationToken);

        return allMovies;
    }

    public async Task RemovePreviousMonthPremieresAsync()
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        var startOfCurrentMonth = new DateOnly(now.Year, now.Month, 1);
        var startOfPreviousMonth = startOfCurrentMonth.AddMonths(-1);
        var endOfPreviousMonth = startOfCurrentMonth.AddDays(-1);

        var oldPremieres = _cxt.Movies
            .Where(p => p.PremiereRu >= startOfPreviousMonth && p.PremiereRu <= endOfPreviousMonth)
            .ToList();

        _cxt.Movies.RemoveRange(oldPremieres);
        await _cxt.SaveChangesAsync();
    }
}
