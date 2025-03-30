using System.ComponentModel.DataAnnotations;

namespace HealthMed.Application.DTOs;

public class EditarHorarioDto
{
    [Required]
    public DateTime DataHora { get; set; }
    public DateTime DataHoraFim { get; set; }

    [Required]
    [RegularExpression("^(disponivel|ocupado)$", ErrorMessage = "Status inválido.")]
    public string Status { get; set; } = "disponivel";
}