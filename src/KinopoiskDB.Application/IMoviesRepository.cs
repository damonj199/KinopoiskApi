using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Application;

public interface IMoviesRepository
{
    Task<IReadOnlyList<Movie>> AddMoviesAsync(IReadOnlyList<Movie> movies, CancellationToken cancellationToken);
    Task<IReadOnlyList<Movie>> GetAllMoviesAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<Movie>> GetMoviesByFilterAsync(string genres, string countries, CancellationToken cancellationToken);
    Task<IReadOnlyList<Movie>> GetPremieresForMonthAsync(DateOnly premiereRuStart, DateOnly premiereRuEnad, CancellationToken cancellationToken);
    Task RemovePreviousMonthPremieresAsync();
    Task<IReadOnlyList<Movie>> SearchMoviesByNameAsync(string title, CancellationToken cancellationToken);
}
