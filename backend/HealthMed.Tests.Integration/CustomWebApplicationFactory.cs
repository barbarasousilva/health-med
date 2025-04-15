using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using Npgsql;
using DotNetEnv;
using System.IO;
using System;

namespace HealthMed.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        LoadEnvironmentVariables();

        var jwtSecretTest = GetRequiredEnvironmentVariable("JWT_SECRET_TEST");
        var connectionString = GetRequiredEnvironmentVariable("DB_CONNECTION_STRING_TEST");

        Environment.SetEnvironmentVariable("JWT_SECRET", jwtSecretTest);

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["JWT_SECRET"] = jwtSecretTest
            });
        });

        // Configura o serviço de banco de dados
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDbConnection));
            services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));
        });
    }

    // Carrega o .env no ambiente
    private static void LoadEnvironmentVariables()
    {
        var basePath = AppContext.BaseDirectory;
        var current = new DirectoryInfo(basePath);
        while (current != null && !File.Exists(Path.Combine(current.FullName, ".env")))
        {
            current = current.Parent;
        }

        var envPath = Path.Combine(current?.FullName ?? "", ".env");
        if (File.Exists(envPath))
            DotNetEnv.Env.Load(envPath);
        else
            throw new FileNotFoundException($".env não encontrado a partir de {basePath}");
    }

    // Obtem a variável de ambiente ou lança erro
    private static string GetRequiredEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name)
            ?? throw new InvalidOperationException($"Variável de ambiente obrigatória '{name}' não está configurada.");
    }
}
