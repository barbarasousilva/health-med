using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    public async Task<string?> AutenticarPacienteAsync(string cpfOuEmail, string senha)
    {
        var paciente = await _repository.ObterPorCpfOuEmailAsync(cpfOuEmail);

        if (paciente == null || !BCrypt.Net.BCrypt.Verify(senha, paciente.SenhaHash))
            return null;


        var jwtSecret = _configuration["JWT_SECRET"];
        if (string.IsNullOrEmpty(jwtSecret))
            throw new InvalidOperationException("JWT_SECRET não configurado.");

        var keyBytes = Convert.FromHexString(jwtSecret);
        var key = new SymmetricSecurityKey(keyBytes) { KeyId = "chave-token" };
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, paciente.Id.ToString()),
            new Claim(ClaimTypes.Name, paciente.Nome),
            new Claim("cpf", paciente.Cpf),
            new Claim(ClaimTypes.Role, "paciente")
        };

        var tokenDescriptor = new JwtSecurityToken(
        issuer: "HealthMed",
        audience: "HealthMed",
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return tokenString;

    }

    public async Task<Guid> RegistrarPacienteAsync(Paciente paciente, string senha)
    {
        var cpf = new string(paciente.Cpf.Where(char.IsDigit).ToArray());

        if (await _repository.ObterPorEmailOuCpfAsync(paciente.Email, cpf) is not null)
            throw new InvalidOperationException("Já existe um paciente com esse CPF ou e-mail.");


        var hash = BCrypt.Net.BCrypt.HashPassword(senha);

        var pacienteFinal = new Paciente(
            paciente.Id,
            paciente.Nome,
            cpf,
            paciente.Email,
            hash
        );

        await _repository.AdicionarAsync(pacienteFinal);
        return pacienteFinal.Id;
    }


}
