using HealthMed.Application.DTOs;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;

namespace HealthMed.Application.Services;

public class HorarioDisponivelService : IHorarioDisponivelService
{
    private readonly IHorarioDisponivelRepository _horarioDisponivel;

    public HorarioDisponivelService(IHorarioDisponivelRepository horarioDisponivel)
    {
        _horarioDisponivel = horarioDisponivel;
    }

    public async Task<IEnumerable<HorarioDisponivel>> ListarPorMedicoAsync(Guid medicoId)
    {
        return await _horarioDisponivel.ListarPorMedicoAsync(medicoId);
    }

    public async Task<Guid> AdicionarAsync(Guid medicoId, DateTime dataHora, DateTime dataHoraFim, StatusHorario status)
    {
        var horario = new HorarioDisponivel(Guid.NewGuid(), medicoId, dataHora, dataHoraFim, status);
        await _horarioDisponivel.AdicionarAsync(horario);
        return horario.Id;
    }

    public async Task AtualizarAsync(Guid id, Guid medicoId, DateTime dataHora, DateTime dataHoraFim, StatusHorario status)
    {
        var horario = await _horarioDisponivel.ObterPorIdAsync(id);

        if (horario == null || horario.MedicoId != medicoId)
            throw new UnauthorizedAccessException("Horário não encontrado ou não pertence ao médico logado.");

        horario.Atualizar(dataHora, dataHoraFim, status);
        await _horarioDisponivel.AtualizarAsync(horario);
    }

    public async Task AbrirAgendaAsync(Guid medicoId, DateTime data, TimeSpan duracao)
    {
        var inicioDia = data.Date.AddHours(8);
        var fimDia = data.Date.AddHours(18);

        var horariosExistentes = (await _horarioDisponivel.ListarPorMedicoAsync(medicoId))
            .Select(h => h.DataHora)
            .ToHashSet();

        for (var inicio = inicioDia; inicio.Add(duracao) <= fimDia; inicio = inicio.Add(duracao))
        {
            if (horariosExistentes.Contains(inicio)) continue;

            var horario = new HorarioDisponivel(Guid.NewGuid(), medicoId, inicio, inicio.Add(duracao), StatusHorario.Disponivel);
            await _horarioDisponivel.AdicionarAsync(horario);
        }
    }
    public async Task RemoverAsync(Guid id, Guid medicoId)
    {
        var horario = await _horarioDisponivel.ObterPorIdAsync(id);
        if (horario == null || horario.MedicoId != medicoId)
            throw new UnauthorizedAccessException("Horário não encontrado ou não pertence ao médico logado.");
        await _horarioDisponivel.RemoverAsync(id);
    }

}
