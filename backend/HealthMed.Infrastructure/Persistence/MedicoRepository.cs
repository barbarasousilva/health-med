using Dapper;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Interfaces;
using System.Data;

namespace HealthMed.Infrastructure.Persistence;

public class MedicoRepository : IMedicoRepository
{
    private readonly IDbConnection _connection;

    public MedicoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Medico?> ObterPorCRMAsync(string crm)
    {
        var sql = @"SELECT id, nome, crm, especialidade, senhaHash
                    FROM medicos
                    WHERE crm = @CRM";

        return await _connection.QueryFirstOrDefaultAsync<Medico>(sql, new { CRM = crm });
    }

    public async Task AdicionarAsync(Medico medico)
    {
        var sql = @"INSERT INTO medicos (id, nome, crm, especialidade, senhaHash)
                    VALUES (@Id, @Nome, @CRM, @Especialidade, @SenhaHash)";

        await _connection.ExecuteAsync(sql, medico);
    }
}
