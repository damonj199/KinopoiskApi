namespace KinopoiskDB.Core.Models;

public class Movies
{
    public Guid Id { get; set; }
    public int KinopoiskId { get; set; }
    public string NameRu { get; set; }
    public string? NameOriginal { get; set; }
    public string? PosterUrl { get; set; }
    public DateTime? Year { get; set; }
    public List<Countries> Countries { get; set; }
    public List<Genres> Genres { get; set; }
}
