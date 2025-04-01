using HealthMed.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class CadastrarHorarioDto
{
    [Required]
    public DateTime DataHora { get; set; }

    [Required]
    public DateTime DataHoraFim { get; set; }

    [Required]
    public StatusHorario Status { get; set; } = StatusHorario.Disponivel;
}
