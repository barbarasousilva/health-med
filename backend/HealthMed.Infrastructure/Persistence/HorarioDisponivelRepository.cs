using System.Data;
using System.Text;
using Dapper;
using HealthMed.Application.DTOs;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;

namespace HealthMed.Infrastructure.Persistence;

public class HorarioDisponivelRepository : IHorarioDisponivelRepository
{
    private readonly IDbConnection _connection;

    public HorarioDisponivelRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task AdicionarAsync(HorarioDisponivel horario)
    {
        const string query = @"
            INSERT INTO horariosdisponiveis (id, medicoid, datahora, datahorafim, status)
            VALUES (@Id, @MedicoId, @DataHora, @DataHoraFim, @Status);
        ";

        await _connection.ExecuteAsync(query, horario);
    }

    public async Task<IEnumerable<HorarioDisponivel>> ListarPorMedicoAsync(Guid medicoId)
    {
        const string query = @"
            SELECT * FROM horariosdisponiveis
            WHERE medicoid = @MedicoId
            ORDER BY datahora;
        ";

        return await _connection.QueryAsync<HorarioDisponivel>(query, new { MedicoId = medicoId });
    }

    public async Task<HorarioDisponivel?> ObterPorIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM horariosdisponiveis
            WHERE id = @Id;
        ";

        return await _connection.QueryFirstOrDefaultAsync<HorarioDisponivel>(query, new { Id = id });
    }

    public async Task AtualizarAsync(HorarioDisponivel horario)
    {
        const string query = @"
            UPDATE horariosdisponiveis
            SET datahora = @DataHora, datahorafim = @DataHoraFim, status = @Status
            WHERE id = @Id;
        ";

        await _connection.ExecuteAsync(query, horario);
    }

    public async Task RemoverAsync(Guid id)
    {
        const string query = @"
            DELETE FROM horariosdisponiveis
            WHERE id = @Id;
        ";

        await _connection.ExecuteAsync(query, new { Id = id });
    }
    public async Task<IEnumerable<object>> BuscarHorariosAsync(
           DateOnly? data,
           StatusHorario? horario,
           string? especialidade,
           Guid? medicoId)
    {
        var sql = new StringBuilder(@"
                SELECT 
                    h.Id, h.DataHora, h.DataHoraFim,
                    m.Nome AS NomeMedico, m.Especialidade
                FROM HorariosDisponiveis h
                INNER JOIN Medicos m ON h.MedicoId = m.Id
                WHERE h.Status = @StatusDisponivel
            ");

        var parametros = new DynamicParameters();
        parametros.Add("StatusDisponivel", StatusHorario.Disponivel);

        if (data.HasValue)
        {
            sql.Append(" AND CAST(h.DataHora AS DATE) = @Data");
            parametros.Add("Data", data.Value.ToDateTime(new TimeOnly()).Date);
        }

        if (horario.HasValue)
        {
            sql.Append(" AND CAST(h.DataHora AS TIME) = @Horario");
            parametros.Add("Horario", (TimeSpan)(object)horario.Value);
        }

        if (!string.IsNullOrWhiteSpace(especialidade))
        {
            sql.Append(" AND m.Especialidade = @Especialidade");
            parametros.Add("Especialidade", especialidade);
        }

        if (medicoId.HasValue)
        {
            sql.Append(" AND h.MedicoId = @MedicoId");
            parametros.Add("MedicoId", medicoId);
        }

        var resultado = await _connection.QueryAsync<BuscarMedicosDto>(sql.ToString(), parametros);
        return resultado.Cast<object>();
    }
}

