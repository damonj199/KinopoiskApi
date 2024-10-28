using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Application;

public interface IKinopoiskService
{
    Task<IReadOnlyList<MovieDto>> SearchMoviesAsync(string title, int? year, CancellationToken cancellationToken);
    Task<IReadOnlyList<MovieDto>> GetPremieresAsync(PremiereRequest premiereRequest, CancellationToken cancellationToken);
    Task<IReadOnlyList<MovieDto>> SyncPremieresBackgrondAsync(int year, string? month, CancellationToken cancellationToken);
    Task<IReadOnlyList<MovieDto>> SyncMovieDataAsync(string jsonResponse);
    Task<int> AddFilms(PremiereRequest premiereRequest, CancellationToken cancellationToken);
}
