using System.Data;
using System.Text;
using DotNetEnv;
using HealthMed.Api.Extensions;
using HealthMed.Infrastructure.Persistence;
using HealthMed.Infrastructure.Auth;
using HealthMed.Application.Services;
using HealthMed.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Repositories;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);


// Configuração do segredo do JWT
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new Exception("JWT_SECRET não configurado!");
Console.WriteLine("JWT_SECRET no Program.cs: " + builder.Configuration["JWT_SECRET"]);

var keyBytes = Convert.FromHexString(jwtSecret);
var key = new SymmetricSecurityKey(keyBytes) { KeyId = "chave-token" };

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Configuração do Swagger
builder.Services.AddSwaggerWithJwt();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configuração do JWT e autenticação
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "HealthMed",
            ValidAudience = "HealthMed",
            RoleClaimType = ClaimTypes.Role,
            IssuerSigningKey = key
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Falha na autenticação do JWT: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"Token validado: {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });


// Registros de serviços
builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();
builder.Services.AddScoped<ConsultaService>();
builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<IHorarioDisponivelRepository, HorarioDisponivelRepository>();
builder.Services.AddScoped<IHorarioDisponivelService, HorarioDisponivelService>();
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var factory = sp.GetRequiredService<IDbConnectionFactory>();
    return factory.CreateConnection();
});

// Construção do app
var app = builder.Build();

// Configurações de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores
app.MapControllers();

app.Run();

public partial class Program { }
