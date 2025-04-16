using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data;
using Npgsql;
using DotNetEnv;
using System.IO;
using System;
using HealthMed.Domain.Interfaces;
using HealthMed.Infrastructure.Auth;
using Infrastructure.Repositories;
using HealthMed.Infrastructure.Persistence;
using HealthMed.Application.Services;

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
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT_SECRET"] = jwtSecretTest
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDbConnection));
            services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));

            services.RemoveAll(typeof(IPacienteService));
            services.RemoveAll(typeof(IPacienteRepository));
            services.RemoveAll(typeof(IMedicoRepository));
            services.RemoveAll(typeof(IAuthService));
            services.RemoveAll(typeof(IConsultaService));
            services.RemoveAll(typeof(IConsultaRepository));
            services.RemoveAll(typeof(IHorarioDisponivelRepository));
            services.RemoveAll(typeof(IHorarioDisponivelService));
            services.RemoveAll(typeof(IDbConnectionFactory));

            services.AddScoped<IPacienteService, PacienteService>();
            services.AddScoped<IPacienteRepository, PacienteRepository>();
            services.AddScoped<IMedicoRepository, MedicoRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IConsultaService, ConsultaService>();
            services.AddScoped<IConsultaRepository, ConsultaRepository>();
            services.AddScoped<IHorarioDisponivelRepository, HorarioDisponivelRepository>();
            services.AddScoped<IHorarioDisponivelService, HorarioDisponivelService>();
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        });
    }

    private static void LoadEnvironmentVariables()
    {
        var basePath = Directory.GetCurrentDirectory();
        var current = new DirectoryInfo(basePath);

        while (current != null && !File.Exists(Path.Combine(current.FullName, ".env.test")))
            current = current.Parent;

        var envPath = Path.Combine(current?.FullName ?? "", ".env.test");

        if (!File.Exists(envPath))
            throw new FileNotFoundException($".env.test não encontrado a partir de {basePath}");

        foreach (var line in File.ReadLines(envPath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
            }
        }
    }

    private static string GetRequiredEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name)
            ?? throw new InvalidOperationException($"Variável de ambiente obrigatória '{name}' não está configurada.");
    }
}
