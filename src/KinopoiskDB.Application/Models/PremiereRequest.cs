using KinopoiskDB.Core.Enum;

namespace KinopoiskDB.Application.Models;

public class PremiereRequest
{
    /// <summary>
    /// Год релиза
    /// </summary>
    public int Year { get; set; }
    /// <summary>
    /// Месяц релиза
    /// </summary>
    public Month Month { get; set; }
}
