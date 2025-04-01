using Domain.Entities;

namespace HealthMed.Domain.Interfaces;

public interface IConsultaRepository
{
    Task AdicionarAsync(Consulta consulta);
    Task<Consulta?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Consulta>> ObterPorMedicoAsync(Guid idMedico);
    Task<IEnumerable<Consulta>> ObterPorPacienteAsync(Guid idPaciente);
    Task AtualizarStatusAsync(Guid idConsulta, string novoStatus, string? justificativa = null, DateTime? dataResposta = null, DateTime? dataCancelamento = null);
    Task<bool> HorarioJaAgendadoAsync(Guid idHorarioDisponivel);
}
