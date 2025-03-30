using HealthMed.Application.DTOs;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Interfaces;

namespace HealthMed.Application.Services;

public interface IHorarioDisponivelService
{
    Task<IEnumerable<HorarioDisponivel>> ListarPorMedicoAsync(Guid medicoId);
    Task<Guid> AdicionarAsync(Guid medicoId, CadastrarHorarioDto dto);
    Task AtualizarAsync(Guid id, Guid medicoId, EditarHorarioDto dto);
    Task RemoverAsync(Guid id, Guid medicoId);
    Task AbrirAgendaAsync(Guid medicoId, AbrirAgendaDto dto);
}

public class HorarioDisponivelService : IHorarioDisponivelService
{
    private readonly IHorarioDisponivelRepository _repository;

    public HorarioDisponivelService(IHorarioDisponivelRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HorarioDisponivel>> ListarPorMedicoAsync(Guid medicoId)
    {
        return await _repository.ListarPorMedicoAsync(medicoId);
    }

    public async Task<Guid> AdicionarAsync(Guid medicoId, CadastrarHorarioDto dto)
    {
        var horario = new HorarioDisponivel(Guid.NewGuid(), medicoId, dto.DataHora, dto.DataHoraFim, dto.Status);
        await _repository.AdicionarAsync(horario);
        return horario.Id;
    }

    public async Task AtualizarAsync(Guid id, Guid medicoId, EditarHorarioDto dto)
    {
        var horario = await _repository.ObterPorIdAsync(id);

        if (horario == null || horario.MedicoId != medicoId)
            throw new UnauthorizedAccessException("Horário não encontrado ou não pertence ao médico logado.");

        horario.Atualizar(dto.DataHora, dto.DataHoraFim, dto.Status);
        await _repository.AtualizarAsync(horario);
    }

    public async Task RemoverAsync(Guid id, Guid medicoId)
    {
        var horario = await _repository.ObterPorIdAsync(id);

        if (horario == null || horario.MedicoId != medicoId)
            throw new UnauthorizedAccessException("Horário não encontrado ou não pertence ao médico logado.");

        await _repository.RemoverAsync(id);
    }

    public async Task AbrirAgendaAsync(Guid medicoId, AbrirAgendaDto dto)
    {
        var duracao = (int)dto.Duracao;
        var inicioDia = dto.Data.Date.AddHours(8);
        var fimDia = dto.Data.Date.AddHours(18);

        var horariosExistentes = (await _repository.ListarPorMedicoAsync(medicoId))
            .Select(h => h.DataHora)
            .ToHashSet();

        for (var inicio = inicioDia; inicio.AddMinutes(duracao) <= fimDia; inicio = inicio.AddMinutes(duracao))
        {
            if (horariosExistentes.Contains(inicio)) continue;

            var horario = new HorarioDisponivel(Guid.NewGuid(), medicoId, inicio, inicio.AddMinutes(duracao), "disponivel");
            await _repository.AdicionarAsync(horario);
        }
    }
}
