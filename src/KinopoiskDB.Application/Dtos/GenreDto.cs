namespace KinopoiskDB.Application.Dtos;

public record GenreDto
{
    /// <summary>
    /// Жанры по которым можно фильтровать
    /// </summary>
    public string Genre { get; init; } = default!;
}
