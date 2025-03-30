using HealthMed.Application.DTOs;

namespace HealthMed.Application.Services;

public interface IAuthService
{
    Task<string?> AutenticarMedicoAsync(LoginMedicoDto loginDto);
}
