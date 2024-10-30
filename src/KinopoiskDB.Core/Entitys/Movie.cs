namespace KinopoiskDB.Core.Models;

public class Movie
{
    public Guid Id { get; set; }
    public long KinopoiskId { get; set; }
    public string? NameRu { get; set; }
    public string? NameEn { get; set; }
    public string? NameOriginal { get; set; }
    public int? Year { get; set; }
    public string? PosterUrl { get; set; }
    public string? Description { get; set; }
    public DateTime? PremiereRu { get; set; }
    public ICollection<Country> Countries { get; set; } = [];
    public ICollection<Genre> Genres { get; set; } = [];
}
