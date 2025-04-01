using HealthMed.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class EditarHorarioDto
{
    [Required]
    public DateTime DataHora { get; set; }

    public DateTime DataHoraFim { get; set; }

    [Required]
    public StatusHorario Status { get; set; } = StatusHorario.Disponivel;
}
