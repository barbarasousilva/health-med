using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace HealthMed.Api.Infrastructure
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration["DB_CONNECTION_STRING"];
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }

    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
