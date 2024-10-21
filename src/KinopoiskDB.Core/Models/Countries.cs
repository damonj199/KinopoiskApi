namespace KinopoiskDB.Core.Models;

public class Countries
{
    public int Id { get; set; }
    public string Country { get; set; }
    public List<Movies> Movies { get; set; }
}
