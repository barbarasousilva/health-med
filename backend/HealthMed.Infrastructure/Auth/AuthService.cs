using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthMed.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HealthMed.Application.Services;

public class AuthService : IAuthService
{
    private readonly IMedicoRepository _medicoRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IMedicoRepository medicoRepository, IConfiguration configuration)
    {
        _medicoRepository = medicoRepository;
        _configuration = configuration;
    }

    public async Task<string?> AutenticarMedicoAsync(string crm, string senha)
    {
        var medico = await _medicoRepository.ObterPorCRMAsync(crm);

        if (medico == null || !BCrypt.Net.BCrypt.Verify(senha, medico.SenhaHash))
            return null;

        var jwtSecret = _configuration["JWT_SECRET"];
        if (string.IsNullOrEmpty(jwtSecret))
            throw new InvalidOperationException("JWT_SECRET não configurado.");

        var keyBytes = Encoding.UTF8.GetBytes(jwtSecret);
        var key = new SymmetricSecurityKey(keyBytes) { KeyId = "chave-token" };
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("id", medico.Id.ToString()),
            new Claim(ClaimTypes.Name, medico.Nome),
            new Claim("crm", medico.CRM),
            new Claim(ClaimTypes.Role, "medico")
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
}
