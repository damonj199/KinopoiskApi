using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Application.Models;
using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Application;

public interface IKinopoiskService
{
    Task<IReadOnlyList<MovieDto>> SearchMoviesByNameAsync(string title, CancellationToken cancellationToken);
    Task<IReadOnlyList<MovieDto>> GetPremieresForMonthAsync(PremiereRequest premiereRequest, CancellationToken cancellationToken);
    Task<IReadOnlyList<Movie>> SyncPremieresBackgrondAsync(int year, string? month, CancellationToken cancellationToken);
    Task<int> AddFilms(PremiereRequest premiereRequest, CancellationToken cancellationToken);
    Task<IReadOnlyList<MovieDto>> GetMoviesByFilterAsync(MovieRequest movieRequest, CancellationToken cancellationToken);
    Task<IReadOnlyList<MovieDto>> GetAllMoviesAsync(PagedRequest<MovieDto> pageRes, SortRequest<MovieDto> sortRequest, CancellationToken cancellationToken);
}
