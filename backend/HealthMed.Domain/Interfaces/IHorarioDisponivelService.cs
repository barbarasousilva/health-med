using HealthMed.Domain.Entities;
using HealthMed.Domain.Enums;

namespace HealthMed.Application.Services;

public interface IHorarioDisponivelService
{
    Task<IEnumerable<HorarioDisponivel>> ListarPorMedicoAsync(Guid medicoId);

    Task<Guid> AdicionarAsync(Guid medicoId, DateTime dataHora, DateTime dataHoraFim, StatusHorario status);

    Task AtualizarAsync(Guid id, Guid medicoId, DateTime dataHora, DateTime dataHoraFim, StatusHorario status);

    Task RemoverAsync(Guid id, Guid medicoId);

    Task AbrirAgendaAsync(Guid medicoId, DateTime data, TimeSpan duracao);
}
