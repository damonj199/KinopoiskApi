namespace KinopoiskDB.Core.Dtos;

public class MoviesDto
{
    public Guid Id { get; set; }
    public int KinopoiskId { get; set; }
    public string NameRu { get; set; }
    public string? NameOriginal { get; set; }
    public string? PosterUrl { get; set; }
    public DateTime? Year { get; set; }
    public List<CountriesDto> Countries { get; set; }
    public List<GenresDto> Genres { get; set; }
}
