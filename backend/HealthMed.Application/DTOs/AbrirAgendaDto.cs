using HealthMed.Domain.Enums;

namespace HealthMed.Application.DTOs;

public class AbrirAgendaDto
{
    public DateTime Data { get; set; }
    public DuracaoConsulta Duracao { get; set; }
}
