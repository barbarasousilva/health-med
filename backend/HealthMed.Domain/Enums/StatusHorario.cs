using System.Text.Json.Serialization;

namespace HealthMed.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusHorario
{
    Disponivel,
    Ocupado
}
