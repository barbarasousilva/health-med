using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data;
using Npgsql;

namespace HealthMed.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration((context, config) =>
		{
			var integrationSettings = new Dictionary<string, string>
			{
				{ "JWT_SECRET", "00112233445566778899AABBCCDDEEFF00112233445566778899AABBCCDDEEFF" }
			};
			config.AddInMemoryCollection(integrationSettings);
		});

		builder.ConfigureServices(services =>
		{
			services.RemoveAll(typeof(IDbConnection));
			services.AddScoped<IDbConnection>(_ =>
				new NpgsqlConnection("Host=localhost;Port=5433;Username=postgres;Password=postgres;Database=healthmed_test")
			);
		});
	}
}
