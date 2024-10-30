namespace KinopoiskDB.Core.Models;

public class Genre
{
    public int Id { get; set; }
    public string Value { get; set; } = default!;
    public ICollection<Movie> Movies { get; set; } = [];
}
