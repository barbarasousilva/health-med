using HealthMed.Domain.Enums;

public static class DuracaoConsultaExtensions
{
    public static TimeSpan ToTimeSpan(this DuracaoConsulta duracao)
    {
        return TimeSpan.FromMinutes((int)duracao);
    }
}
