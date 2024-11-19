using System.Text.Json.Serialization;

namespace KinopoiskDB.Core.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortableFields
{
    KinopoiskId,
    NameRu,
    NameEn,
    Year,
    PremiereRu
}
