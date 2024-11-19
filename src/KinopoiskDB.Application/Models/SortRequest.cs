using KinopoiskDB.Core.Enum;

namespace KinopoiskDB.Application.Models;

public class SortRequest<T>
{
    /// <summary>
    /// Здесь можно задать по какому свойству сортировать
    /// </summary>
    public SortableFields SortField { get; set; } = SortableFields.KinopoiskId;
    public SortOrder SortOrder { get; set; } = SortOrder.ASC;
}
