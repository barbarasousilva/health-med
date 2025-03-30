using HealthMed.Domain.Entities;

namespace HealthMed.Domain.Interfaces;

public interface IHorarioDisponivelRepository
{
    Task AdicionarAsync(HorarioDisponivel horario);
    Task<IEnumerable<HorarioDisponivel>> ListarPorMedicoAsync(Guid medicoId);
    Task<HorarioDisponivel?> ObterPorIdAsync(Guid id);
    Task AtualizarAsync(HorarioDisponivel horario);
    Task RemoverAsync(Guid id);
}