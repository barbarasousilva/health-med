using Dapper;
using Domain.Entities;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;
using System.Data;

namespace Infrastructure.Repositories;

public class ConsultaRepository : IConsultaRepository
{
    private readonly IDbConnection _connection;

    public ConsultaRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    public async Task<IEnumerable<Consulta>> ListarPorStatusAsync(Guid medicoId, StatusConsulta status)
    {
        var query = @"SELECT * FROM Consultas WHERE MedicoId = @MedicoId AND Status = @Status";
        return await _connection.QueryAsync<Consulta>(query, new { MedicoId = medicoId, Status = status.ToString() });
    }


    public async Task AdicionarAsync(Consulta consulta)
    {
        var sql = @"INSERT INTO consultas (
                        id, pacienteid, medicoid, horarioid, status, justificativacancelamento, criadoem
                    ) VALUES (
                        @Id, @IdPaciente, @IdMedico, @IdHorarioDisponivel, @Status, @JustificativaCancelamento, @DataAgendamento
                    )";

        await _connection.ExecuteAsync(sql, new
        {
            consulta.Id,
            consulta.IdPaciente,
            consulta.IdMedico,
            consulta.IdHorarioDisponivel,
            Status = consulta.Status.ToString(),
            consulta.JustificativaCancelamento,
            consulta.DataAgendamento
        });
    }

    public async Task<Consulta?> ObterPorIdAsync(Guid id)
    {
        var sql = @"SELECT * FROM consultas WHERE id = @id";

        var consulta = await _connection.QuerySingleOrDefaultAsync<ConsultaRaw>(sql, new { id });

        return consulta == null ? null : MapearConsulta(consulta);
    }

    public async Task<IEnumerable<Consulta>> ObterPorMedicoAsync(Guid idMedico)
    {
        var sql = @"SELECT * FROM consultas WHERE medicoid = @idMedico";

        var rows = await _connection.QueryAsync<ConsultaRaw>(sql, new { idMedico });
        return rows.Select(MapearConsulta);
    }

    public async Task<IEnumerable<Consulta>> ObterPorPacienteAsync(Guid idPaciente)
    {
        var sql = @"SELECT * FROM consultas WHERE pacienteid = @idPaciente";

        var rows = await _connection.QueryAsync<ConsultaRaw>(sql, new { idPaciente });
        return rows.Select(MapearConsulta);
    }

    public async Task AtualizarStatusAsync(Guid idConsulta, string novoStatus, string? justificativa = null, DateTime? dataResposta = null, DateTime? dataCancelamento = null)
    {
        var sql = @"UPDATE consultas
                    SET status = @Status,
                        justificativacancelamento = @Justificativa,
                        datarespostamedico = @DataResposta,
                        datacancelamento = @DataCancelamento
                    WHERE id = @Id";

        await _connection.ExecuteAsync(sql, new
        {
            Id = idConsulta,
            Status = novoStatus,
            Justificativa = justificativa,
            DataResposta = dataResposta,
            DataCancelamento = dataCancelamento
        });
    }

    public async Task<bool> HorarioJaAgendadoAsync(Guid idHorarioDisponivel)
    {
        var sql = @"SELECT COUNT(*) FROM consultas
                    WHERE horarioid = @idHorarioDisponivel
                      AND status IN ('Pendente', 'Aceita', 'Recusada')";

        var count = await _connection.ExecuteScalarAsync<int>(sql, new { idHorarioDisponivel });
        return count > 0;
    }

    private Consulta MapearConsulta(ConsultaRaw raw)
    {
        return new Consulta
        {
            Id = raw.Id,
            IdPaciente = raw.PacienteId,
            IdMedico = raw.MedicoId,
            IdHorarioDisponivel = raw.HorarioDisponivelId,
            DataAgendamento = raw.DataAgendamento,
            Status = Enum.Parse<StatusConsulta>(raw.Status, ignoreCase: true),
            JustificativaCancelamento = raw.JustificativaCancelamento,
            DataRespostaMedico = raw.DataRespostaMedico,
            DataCancelamento = raw.DataCancelamento
        };
    }

    private class ConsultaRaw
    {
        public Guid Id { get; set; }
        public Guid PacienteId { get; set; }
        public Guid MedicoId { get; set; }
        public Guid HorarioDisponivelId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? JustificativaCancelamento { get; set; }
        public DateTime DataAgendamento { get; set; }
        public DateTime? DataRespostaMedico { get; set; }
        public DateTime? DataCancelamento { get; set; }
    }

}
