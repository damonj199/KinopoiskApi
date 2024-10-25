using KinopoiskDB.Core.Enum;
using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Application;

public interface IMoviesRepository
{
    Task<List<Movie>> AddMoviesAsync(List<Movie> movies);
    Task<List<Movie>> GetPremieresAsync(int year, Month month, CancellationToken cancellationToken);
}
