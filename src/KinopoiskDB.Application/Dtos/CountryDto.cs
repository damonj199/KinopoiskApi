namespace KinopoiskDB.Application.Dtos;

public record CountryDto
{
    /// <summary>
    /// Cтраны которые снимали фильм
    /// </summary>
    public string Country { get; init; } = default!;
}