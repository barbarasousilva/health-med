using HealthMed.Domain.Entities;
using HealthMed.Domain.Filters;

namespace HealthMed.Application.Services
{
    public interface IMedicoService
    {
        Task<Guid> RegistrarMedicoAsync(Medico medico);
        Task<IEnumerable<Medico>> BuscarAsync(FiltroMedico filtro);
    }
}
