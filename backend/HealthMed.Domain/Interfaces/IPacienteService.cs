using HealthMed.Domain.Entities;

namespace HealthMed.Domain.Interfaces;

public interface IPacienteService
{
    Task<string?> AutenticarPacienteAsync(string cpfOuEmail, string senha);
    Task<Guid> RegistrarPacienteAsync(Paciente paciente);
}
