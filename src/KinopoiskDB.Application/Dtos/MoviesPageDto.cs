namespace KinopoiskDB.Application.Dtos;

public record MoviesPageDto
{
    public int Total { get; init; }
    public int TotalPages { get; init; }
    public int Page { get; init; }
    public List<MovieDto> Items { get; init; } = [];
}
