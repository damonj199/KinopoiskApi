namespace KinopoiskDB.Application.Dtos;

public record class MoviesPageDto
{
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public int Page { get; set; }
    public List<MovieDto> Items { get; set; } = [];
}
