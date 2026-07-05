using System.Text.Json.Serialization;

namespace ExpenseTracker.Api.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExpenseCategory
{
    Food = 1,
    Transport = 2,
    Housing = 3,
    Entertainment = 4,
    Health = 5,
    Other = 999
}
