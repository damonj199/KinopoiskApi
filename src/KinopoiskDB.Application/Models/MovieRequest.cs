namespace KinopoiskDB.Application.Models;

public class MovieRequest
{
    /// <summary>
    /// Жанры фильмов
    /// </summary>
    public string[] Genres { get; set; } = [];
    /// <summary>
    /// Страны которые снимали фильм
    /// </summary>
    public string[] Countries { get; set; } = [];
}
