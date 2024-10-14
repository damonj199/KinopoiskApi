namespace KinopoikDB.Api.Services;

public interface IKinopoiskService
{
    Task<dynamic> SearchMoviesAsync(string title, int? year, string genre);
    Task<dynamic> GetPremieresAsync(DateTime month);
}
