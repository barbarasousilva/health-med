namespace HealthMed.Application.DTOs;

public class RegistrarPacienteDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
