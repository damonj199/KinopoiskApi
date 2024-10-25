using KinopoiskDB.Core.Enum;

namespace KinopoiskDB.Core.Models;

public class Movie
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
    public List<Country> Countries { get; set; } = [];
    public List<Genre> Genres { get; set; } = [];
}
