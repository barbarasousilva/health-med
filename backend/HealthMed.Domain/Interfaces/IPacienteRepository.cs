using HealthMed.Domain.Entities;

namespace HealthMed.Domain.Interfaces;

public interface IPacienteRepository
{
    Task<Paciente?> ObterPorCpfOuEmailAsync(string cpfOuEmail);
    Task<Paciente?> ObterPorEmailOuCpfAsync(string email, string cpf);
    Task AdicionarAsync(Paciente paciente);
}
