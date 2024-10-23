using KinopoiskDB.Application.Dtos;

namespace KinopoiskDB.Application;

public interface IKinopoiskService
{
    Task<IReadOnlyList<MovieDto>> SearchMoviesAsync(string title, int? year, CancellationToken cancellationToken);
    Task<IReadOnlyList<MovieDto>> GetPremieresAsync(int year, int month, CancellationToken cancellationToken);
    Task<IReadOnlyList<MovieDto>> SyncMovieDataAsync(string jsonResponse);
}
