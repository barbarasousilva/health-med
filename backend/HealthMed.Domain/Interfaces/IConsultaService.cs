using HealthMed.Domain.Enums;

namespace HealthMed.Domain.Interfaces
{
    public interface IConsultaService
    {
        Task<IEnumerable<object>> ListarPorStatusAsync(Guid medicoId, StatusConsulta status);
        Task AgendarConsultaAsync(Guid idPaciente, Guid idMedico, Guid idHorarioDisponivel);
        Task CancelarConsultaAsync(Guid idConsulta, string justificativa);
        Task AceitarConsultaAsync(Guid idConsulta);
        Task RecusarConsultaAsync(Guid idConsulta);
    }
}