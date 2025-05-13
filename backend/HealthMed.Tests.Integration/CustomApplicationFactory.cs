using Microsoft.AspNetCore.Hosting;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace HealthMed.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            DotNetEnv.Env.Load(".env");

            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            var dbConnString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(dbConnString))
                throw new Exception("JWT_SECRET ou DB_CONNECTION_STRING não estão definidas no ambiente.");

            Environment.SetEnvironmentVariable("JWT_SECRET", jwtSecret);
            Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", dbConnString);
        }

    }
}
