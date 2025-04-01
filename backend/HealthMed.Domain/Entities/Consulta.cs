using HealthMed.Domain.Entities;
using HealthMed.Domain.Enums;

namespace Domain.Entities;

public class Consulta
{
    public Guid Id { get; set; }
    public Guid IdPaciente { get; set; }
    public Guid IdMedico { get; set; }
    public Guid IdHorarioDisponivel { get; set; }

    public DateTime DataAgendamento { get; set; } = DateTime.UtcNow;
    public StatusConsulta Status { get; set; } = StatusConsulta.Pendente;

    public string? JustificativaCancelamento { get; set; }
    public DateTime? DataRespostaMedico { get; set; }
    public DateTime? DataCancelamento { get; set; }

    public Paciente? Paciente { get; set; }
    public Medico? Medico { get; set; }
    public HorarioDisponivel? Horario { get; set; }
}
