namespace KinopoiskDB.Core.Models;

public class MovieRequest
{
    public string[] Genres { get; set; } = [];
    public string[] Countries { get; set; } = [];
}
