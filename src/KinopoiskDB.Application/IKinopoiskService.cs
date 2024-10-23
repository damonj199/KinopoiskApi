using KinopoiskDB.Application.Dtos;

namespace KinopoiskDB.Application;

public interface IKinopoiskService
{
    Task<List<MoviesDto>> SearchMoviesAsync(string title, int? year, CancellationToken cancellationToken);
    Task<List<MoviesDto>> GetPremieresAsync(int year, int month, CancellationToken cancellationToken);
    Task<List<MoviesDto>> SyncMovieDataAsync(string jsonResponse);
}
