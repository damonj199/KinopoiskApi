namespace KinopoiskDB.Application.Dtos;

public class Genres
{
    public int Id { get; set; }
    public string Genre { get; set; }
    public List<Movies> Movies { get; set; }
}
