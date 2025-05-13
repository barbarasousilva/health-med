using System.Text.Json.Serialization;

namespace HealthMed.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusConsulta
{
    Pendente,
    Aceita,
    Recusada,
    Cancelada
}
