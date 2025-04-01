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
            Id = raw.id,
            IdPaciente = raw.pacienteid,
            IdMedico = raw.medicoid,
            IdHorarioDisponivel = raw.horarioid,
            DataAgendamento = raw.criadoem,
            Status = Enum.Parse<StatusConsulta>(raw.status, ignoreCase: true),
            JustificativaCancelamento = raw.justificativacancelamento,
            DataRespostaMedico = raw.datarespostamedico,
            DataCancelamento = raw.datacancelamento
        };
    }

    private class ConsultaRaw
    {
        public Guid id { get; set; }
        public Guid pacienteid { get; set; }
        public Guid medicoid { get; set; }
        public Guid horarioid { get; set; }
        public string status { get; set; } = string.Empty;
        public string? justificativacancelamento { get; set; }
        public DateTime criadoem { get; set; }
        public DateTime? datarespostamedico { get; set; }
        public DateTime? datacancelamento { get; set; }
    }
}
