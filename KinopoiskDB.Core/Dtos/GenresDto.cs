namespace KinopoiskDB.Core.Dtos;

public class GenresDto
{
    public int Id { get; set; }
    public string Genre { get; set; }
    public List<MoviesDto> Movies { get; set; }
}
