using HealthMed.Domain.Entities;

namespace HealthMed.Domain.Interfaces
{
    public interface IMedicoRepository
    {
        Task<Medico?> ObterPorCRMAsync(string crm);
        Task AdicionarAsync(Medico medico);
    }
}