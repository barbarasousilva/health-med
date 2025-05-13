using HealthMed.Domain.Enums;

namespace HealthMed.Application.DTOs;


public class ConsultaDto
{
    public Guid Id { get; set; }
    public Guid MedicoId { get; set; }
    public Guid PacienteId { get; set; }
    public Guid HorarioDisponivelId { get; set; }
    public StatusConsulta Status { get; set; }
    public DateTime DataAgendamento { get; set; }
    public string JustificativaCancelamento { get; set; } = string.Empty;
}
