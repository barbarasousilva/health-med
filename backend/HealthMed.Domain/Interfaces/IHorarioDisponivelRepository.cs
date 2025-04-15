using HealthMed.Domain.Entities;
using HealthMed.Domain.Enums;

namespace HealthMed.Domain.Interfaces;

public interface IHorarioDisponivelRepository
{
    Task AdicionarAsync(HorarioDisponivel horario);
    Task AtualizarAsync(HorarioDisponivel horario);
    Task<HorarioDisponivel?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<HorarioDisponivel>> ListarPorMedicoAsync(Guid idMedico);
    Task RemoverAsync(Guid id);
    Task<IEnumerable<object>> BuscarHorariosAsync(
            DateOnly? data,
            StatusHorario? horario,
            string? especialidade,
            Guid? medicoId);
}