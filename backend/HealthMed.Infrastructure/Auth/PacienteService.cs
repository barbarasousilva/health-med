using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HealthMed.Infrastructure.Auth;

public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _repository;
    private readonly IConfiguration _configuration;

    public PacienteService(IPacienteRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<string?> AutenticarPacienteAsync(LoginPacienteDto loginDto)
    {
        var paciente = await _repository.ObterPorCpfOuEmailAsync(loginDto.CpfOuEmail);

        if (paciente == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, paciente.SenhaHash))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var key = Convert.FromHexString(_configuration["JWT_SECRET"]!);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, paciente.Id.ToString()),
            new Claim(ClaimTypes.Name, paciente.Nome),
            new Claim("cpf", paciente.Cpf),
            new Claim(ClaimTypes.Role, "paciente")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<Guid> RegistrarPacienteAsync(RegistrarPacienteDto dto)
    {
        var cpf = new string(dto.Cpf.Where(char.IsDigit).ToArray());
        Console.WriteLine($"CPF tratado: {cpf} - Tamanho: {cpf.Length}");


        if (await _repository.ObterPorEmailOuCpfAsync(dto.Email, cpf) is not null)
            throw new InvalidOperationException("Já existe um paciente com esse CPF ou e-mail.");

        var paciente = new Paciente(
            Guid.NewGuid(),
            dto.Nome.Trim(),
            cpf,
            dto.Email.Trim().ToLowerInvariant(),
            BCrypt.Net.BCrypt.HashPassword(dto.Senha)
        );

        await _repository.AdicionarAsync(paciente);
        return paciente.Id;
    }
}
