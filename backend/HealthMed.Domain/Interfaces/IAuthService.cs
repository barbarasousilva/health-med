namespace HealthMed.Domain.Interfaces;

public interface IAuthService
{
    Task<string?> AutenticarMedicoAsync(string crm, string senha);
}