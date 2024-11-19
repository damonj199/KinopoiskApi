namespace KinopoiskDB.Application.Models;

public class PagedRequest<T>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 5;
}
