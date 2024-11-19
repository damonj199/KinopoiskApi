using KinopoiskDB.Application;
using KinopoiskDB.Core.Enum;
using KinopoiskDB.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace KinopoiskDB.Dal.PostgreSQL.Repository;

public sealed class MoviesRepository : BaseRepository, IMoviesRepository
{
    public MoviesRepository(KinopoiskDbContext connectionString) : base(connectionString) { }

    public async Task<IReadOnlyList<Movie>> AddMoviesAsync(IReadOnlyList<Movie> movies, CancellationToken cancellationToken)
    {
        foreach (var movie in movies)
        {
            var exisitgMovie = await Context.Movies
                .FirstOrDefaultAsync(m => m.KinopoiskId == movie.KinopoiskId, cancellationToken);

            _ = (exisitgMovie == null) ? Context.Movies.Add(movie) : Context.Movies.Update(exisitgMovie);
        }
        await Context.SaveChangesAsync(cancellationToken);

        return movies;
    }

    public async Task<IReadOnlyList<Movie>> GetMoviesByFilterAsync(string genres, string countries, CancellationToken cancellationToken)
    {
        var query = Context.Movies.Include(g => g.Genres).Include(c => c.Countries).AsQueryable();

        if (!string.IsNullOrWhiteSpace(genres))
            query = query.Where(m => m.Genres.Any() && m.Genres.Any(g => genres.Contains(g.Value)));

        if (!string.IsNullOrWhiteSpace(countries))
            query = query.Where(m => m.Countries.Any() && m.Countries.Any(c => countries.Contains(c.Value)));

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<Movie>> GetPremieresForMonthAsync(DateOnly premiereRuStart, DateOnly premiereRuEnd, CancellationToken cancellationToken)
    {
        var primieres = await Context.Movies
            .AsNoTracking()
            .Where(m => m.PremiereRu >= premiereRuStart && m.PremiereRu <= premiereRuEnd)
            .Include(g => g.Genres)
            .Include(c => c.Countries)
            .ToListAsync(cancellationToken);

        return primieres;
    }

    public async Task<IReadOnlyList<Movie>> SearchMoviesByNameAsync(string title, CancellationToken cancellationToken)
    {
        var moviesByName = await Context.Movies
            .AsNoTracking()
            .Where(m => m.NameRu.ToLower().Contains(title.ToLower()) || m.NameEn.ToLower().Contains(title.ToLower()))
            .Include(g => g.Genres)
            .Include(c => c.Countries)
            .ToListAsync(cancellationToken);

        return moviesByName;
    }

    public async Task<IReadOnlyList<Movie>> GetAllMoviesAsync(int page, int pageSize, SortableFields sortField, SortOrder order, CancellationToken cancellationToken)
    {
        var allMovies = await Context.Movies
            .AsNoTracking()
            .Sort<Movie>(sortField.ToString(), order)
            .Paginate(page, pageSize)
            .Include(g => g.Genres)
            .Include(c => c.Countries)
            .ToListAsync(cancellationToken);

        return allMovies;
    }
}
