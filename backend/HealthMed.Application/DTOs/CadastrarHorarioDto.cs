using System.ComponentModel.DataAnnotations;

namespace HealthMed.Application.DTOs;

public class CadastrarHorarioDto
{
    [Required]
    public DateTime DataHora { get; set; }
    public DateTime DataHoraFim { get; set; }

    [Required]
    [RegularExpression("^(disponivel|ocupado)$", ErrorMessage = "Status inv�lido.")]
    public string Status { get; set; } = "disponivel";
}