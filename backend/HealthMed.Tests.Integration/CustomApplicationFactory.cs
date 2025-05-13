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

            Environment.SetEnvironmentVariable("JWT_SECRET", "8f3d7234f8c84f189f09a7c4be8b0d2e13b6f5b241f7416abfd3343f6c2f6b2a");
            Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=healthmeddb");
        }

    }
}
