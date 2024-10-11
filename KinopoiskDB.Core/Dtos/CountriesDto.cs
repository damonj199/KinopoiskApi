namespace KinopoiskDB.Core.Dtos;

public class CountriesDto
{
    public int Id { get; set; }
    public string Country { get; set; }
    public List<MoviesDto> Movies { get; set; }

}
