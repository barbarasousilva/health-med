using HealthMed.Domain.Entities;

namespace HealthMed.Domain.Interfaces;

public interface IHorarioDisponivelRepository
{
    Task AdicionarAsync(HorarioDisponivel horario);
    Task AtualizarAsync(HorarioDisponivel horario);
    Task<HorarioDisponivel?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<HorarioDisponivel>> ListarPorMedicoAsync(Guid idMedico);
    Task RemoverAsync(Guid id);
}