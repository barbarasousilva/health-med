namespace HealthMed.Application.DTOs;

public class RegistrarMedicoDto
{
    public string Nome { get; set; } = string.Empty;
    public string CRM { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
}
