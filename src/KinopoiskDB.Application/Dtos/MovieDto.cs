namespace KinopoiskDB.Application.Dtos;

public record MovieDto
{
    public Guid Id { get; init; }
    public long KinopoiskId { get; init; }
    public string? NameRu { get; init; }
    public string? NameEn { get; init; }
    public string? NameOriginal { get; init; }
    public int? Year { get; init; }
    public string? PosterUrl { get; init; }
    public string? Description { get; init; }
    public DateOnly? PremiereRu { get; init; }
    public CountryDto[] Countries { get; init; } = [];
    public GenreDto[] Genres { get; init; } = [];
}
