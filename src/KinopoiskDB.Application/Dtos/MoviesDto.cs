namespace KinopoiskDB.Application.Dtos;

public class MoviesDto
{
    public Guid Id { get; set; }
    public int KinopoiskId { get; set; }
    public string? NameRu { get; set; }
    public string? NameEn { get; set; }
    public string? NameOriginal { get; set; }
    public DateTime? Year { get; set; }
    public string? PosterUrl { get; set; }
    public string? Description { get; set; }
    public List<CountriesDto> Countries { get; set; }
    public List<GenresDto> Genres { get; set; }
}
