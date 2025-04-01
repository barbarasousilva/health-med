using Dapper;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Interfaces;
using System.Data;
using System.Text;

namespace Infrastructure.Repositories;

public class MedicoRepository : IMedicoRepository
{
    private readonly IDbConnection _connection;

    public MedicoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task AdicionarAsync(Medico medico)
    {
        var sql = @"INSERT INTO medicos (
                        id, nome, crm, especialidade, senhahash, cidade, uf
                    ) VALUES (
                        @Id, @Nome, @CRM, @Especialidade, @SenhaHash, @Cidade, @UF
                    )";

        await _connection.ExecuteAsync(sql, new
        {
            medico.Id,
            medico.Nome,
            medico.CRM,
            medico.Especialidade,
            medico.SenhaHash,
            medico.Cidade,
            medico.UF
        });
    }

    public async Task<Medico?> ObterPorCRMAsync(string crm)
    {
        var sql = @"SELECT * FROM medicos WHERE crm = @crm";

        return await _connection.QuerySingleOrDefaultAsync<Medico>(sql, new { crm });
    }

    public async Task<IEnumerable<Medico>> BuscarAsync(FiltroMedico filtro)
    {
        var sql = new StringBuilder("SELECT * FROM medicos WHERE 1=1");
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            sql.Append(" AND nome ILIKE @Nome");
            parameters.Add("Nome", $"%{filtro.Nome}%");
        }

        if (!string.IsNullOrWhiteSpace(filtro.Especialidade))
        {
            sql.Append(" AND especialidade ILIKE @Especialidade");
            parameters.Add("Especialidade", $"%{filtro.Especialidade}%");
        }

        if (!string.IsNullOrWhiteSpace(filtro.Cidade))
        {
            sql.Append(" AND cidade ILIKE @Cidade");
            parameters.Add("Cidade", $"%{filtro.Cidade}%");
        }

        if (!string.IsNullOrWhiteSpace(filtro.UF))
        {
            sql.Append(" AND uf ILIKE @UF");
            parameters.Add("UF", filtro.UF);
        }

        return await _connection.QueryAsync<Medico>(sql.ToString(), parameters);
    }
}
