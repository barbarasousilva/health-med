using HealthMed.Domain.Entities;

namespace HealthMed.Application.Services
{
    public interface IMedicoService
    {
        Task<Guid> RegistrarMedicoAsync(Medico medico);
        Task<IEnumerable<Medico>> BuscarAsync(FiltroMedico filtro);
    }
}
