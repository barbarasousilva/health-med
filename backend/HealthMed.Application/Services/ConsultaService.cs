using Domain.Entities;
using HealthMed.Application.DTOs;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;

namespace HealthMed.Application.Services;

public class ConsultaService : IConsultaService
{
    private readonly IConsultaRepository _consulta;

    public ConsultaService(IConsultaRepository consulta)
    {
        _consulta = consulta;
    }
    public async Task<IEnumerable<Consulta>> ListarPorStatusAsync(Guid medicoId, StatusConsulta status)
    {
        return await _consulta.ListarPorStatusAsync(medicoId, status);
    }

    public async Task<Guid> AgendarConsultaAsync(Guid idPaciente, Guid idMedico, Guid idHorarioDisponivel)
    {
        if (await _consulta.HorarioJaAgendadoAsync(idHorarioDisponivel))
            throw new InvalidOperationException("Este horário já está agendado.");

        var consulta = new Consulta
        {
            Id = Guid.NewGuid(),
            IdPaciente = idPaciente,
            IdMedico = idMedico,
            IdHorarioDisponivel = idHorarioDisponivel,
            Status = StatusConsulta.Pendente,
            DataAgendamento = DateTime.UtcNow
        };

        await _consulta.AdicionarAsync(consulta);
        return consulta.Id;
    }

    public async Task CancelarConsultaAsync(Guid idConsulta, string justificativa)
    {
        await _consulta.AtualizarStatusAsync(
            idConsulta,
            StatusConsulta.Cancelada.ToString(),
            justificativa,
            null,
            DateTime.UtcNow
        );
    }

    public async Task AceitarConsultaAsync(Guid idConsulta)
    {
        await _consulta.AtualizarStatusAsync(
            idConsulta,
            StatusConsulta.Aceita.ToString(),
            null,
            DateTime.UtcNow,
            null
        );
    }

    public async Task RecusarConsultaAsync(Guid idConsulta)
    {
        await _consulta.AtualizarStatusAsync(
            idConsulta,
            StatusConsulta.Recusada.ToString(),
            null,
            DateTime.UtcNow,
            null
        );
    }

}
