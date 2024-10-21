using KinopoiskDB.Application.Dtos;

namespace KinopoikDB.Api.Services;

public interface IKinopoiskService
{
    Task<MoviesDto> SearchMoviesAsync(string title, int? year, CancellationToken cancellationToken);
    Task<MoviesDto> GetPremieresAsync(int year, string month, CancellationToken cancellationToken);
}
