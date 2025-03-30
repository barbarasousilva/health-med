using HealthMed.Application.DTOs;

namespace HealthMed.Application.Services;

public interface IPacienteService
{
    Task<string?> AutenticarPacienteAsync(LoginPacienteDto loginDto);
    Task<Guid> RegistrarPacienteAsync(RegistrarPacienteDto dto);
}
