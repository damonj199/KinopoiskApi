namespace KinopoiskDB.Application.Dtos;

public record PagedResponse<T>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 5;
}
