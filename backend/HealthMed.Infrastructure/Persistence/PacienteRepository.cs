using System.Data;
using Dapper;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Interfaces;

namespace HealthMed.Infrastructure.Persistence;

public class PacienteRepository : IPacienteRepository
{
    private readonly IDbConnection _connection;

    public PacienteRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Paciente?> ObterPorCpfOuEmailAsync(string cpfOuEmail)
    {
        const string query = @"
            SELECT * FROM pacientes 
            WHERE LOWER(email) = LOWER(@Valor) OR cpf = @Valor;
        ";

        return await _connection.QueryFirstOrDefaultAsync<Paciente>(query, new { Valor = cpfOuEmail });
    }

    public async Task<Paciente?> ObterPorEmailOuCpfAsync(string email, string cpf)
    {
        const string query = @"
            SELECT * FROM pacientes 
            WHERE LOWER(email) = LOWER(@Email) OR cpf = @Cpf;
        ";

        return await _connection.QueryFirstOrDefaultAsync<Paciente>(query, new { Email = email, Cpf = cpf });
    }

    public async Task AdicionarAsync(Paciente paciente)
    {
        const string query = @"
            INSERT INTO pacientes (id, nome, cpf, email, senhaHash)
            VALUES (@Id, @Nome, @Cpf, @Email, @SenhaHash);
        ";

        await _connection.ExecuteAsync(query, paciente);
    }
}
