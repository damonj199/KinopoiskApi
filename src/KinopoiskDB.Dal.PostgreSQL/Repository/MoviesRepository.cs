using KinopoiskDB.Application;
using KinopoiskDB.Core.Enum;
using KinopoiskDB.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace KinopoiskDB.Dal.PostgreSQL.Repository;

public class MoviesRepository : BaseRepository, IMoviesRepository
{
    public MoviesRepository(KinopoiskDbContext connectionString) : base(connectionString) { }

    public async Task<List<Movie>> AddMoviesAsync(List<Movie> movies, CancellationToken cancellationToken)
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

    public async Task<IReadOnlyList<Movie>> GetMoviesByFilterAsync(MovieRequest mReq, CancellationToken cancellationToken)
    {
        var query = from m in _cxt.Movies.Include(g => g.Genres).Include(c => c.Countries)
                    select m;

        if (mReq.Genres != null && mReq.Genres.Any())
            query = query.Where(m => m.Genres.All(g => mReq.Genres.Contains(g.Value)));

        if (mReq.Countries != null && mReq.Countries.Any())
            query = query.Where(m => m.Countries.All(c => mReq.Countries.Contains(c.Value)));

        return await query.ToListAsync();
    }

    public async Task<List<Movie>> GetPremieresAsync(DateOnly premiereRuStart, DateOnly premiereRuEnd, CancellationToken cancellationToken)
    {
        var primieres = await _cxt.Movies
            .AsNoTracking()
            .Where(m => m.PremiereRu >= premiereRuStart && m.PremiereRu <= premiereRuEnd)
            .Include(g => g.Genres)
            .Include(c => c.Countries)
            .ToListAsync(cancellationToken);

        return primieres;
    }

    public async Task<List<Movie>> SearchMoviesByNameAsync(string title, CancellationToken cancellationToken)
    {
        var moviesByName = await _cxt.Movies
            .AsNoTracking()
            .Where(m => m.NameRu.ToLower().Contains(title.ToLower()) || m.NameEn.ToLower().Contains(title.ToLower()))
            .Include(g => g.Genres)
            .Include(c => c.Countries)
            .ToListAsync(cancellationToken);

        return moviesByName;
    }
}
