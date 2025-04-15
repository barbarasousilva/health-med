namespace HealthMed.Application.DTOs;

public class LoginPacienteDto
{
    public string CpfOuEmail { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
