using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Application.Dtos;

public class MoviesDto
{
    public Guid Id { get; set; }
    public long KinopoiskId { get; set; }
    public string? NameRu { get; set; }
    public string? NameEn { get; set; }
    public string? NameOriginal { get; set; }
    public int? Year { get; set; }
    public string? PosterUrl { get; set; }
    public string? Description { get; set; }
    public List<Countries>? Countries { get; set; }
    public List<Genres>? Genres { get; set; }
}
