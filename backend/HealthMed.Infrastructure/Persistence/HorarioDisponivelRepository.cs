using System.Data;
using Dapper;
using HealthMed.Domain.Entities;
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
}
