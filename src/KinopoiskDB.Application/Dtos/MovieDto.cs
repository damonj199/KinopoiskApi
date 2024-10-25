using KinopoiskDB.Core.Enum;

namespace KinopoiskDB.Application.Dtos;

public class MovieDto
{
    public Guid Id { get; set; }
    public long KinopoiskId { get; set; }
    public string? NameRu { get; set; }
    public string? NameEn { get; set; }
    public string? NameOriginal { get; set; }
    public int? Year { get; set; }
    public Month? Month { get; set; }
    public string? PosterUrl { get; set; }
    public string? Description { get; set; }
    public CountryDto[] Countries { get; set; } = [];
    public GenreDto[] Genres { get; set; } = [];
}
