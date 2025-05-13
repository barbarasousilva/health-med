using Domain.Entities;
using HealthMed.Domain.Enums;

namespace HealthMed.Domain.Interfaces
{
    public interface IConsultaService
    {
        Task<IEnumerable<Consulta>> ListarPorStatusAsync(Guid medicoId, StatusConsulta status);
        Task <Guid>AgendarConsultaAsync(Guid idPaciente, Guid idMedico, Guid idHorarioDisponivel);
        Task CancelarConsultaAsync(Guid idConsulta, string justificativa);
        Task AceitarConsultaAsync(Guid idConsulta);
        Task RecusarConsultaAsync(Guid idConsulta);
    }
}