using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
using HealthMed.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HealthMed.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly IMedicoRepository _medicoRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IMedicoRepository medicoRepository, IConfiguration configuration)
    {
        _medicoRepository = medicoRepository;
        _configuration = configuration;
    }

    public async Task<string?> AutenticarMedicoAsync(LoginMedicoDto loginDto)
    {
        var medico = await _medicoRepository.ObterPorCRMAsync(loginDto.CRM);

        if (medico == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, medico.SenhaHash))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JWT_SECRET"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, medico.Id.ToString()),
                new Claim(ClaimTypes.Name, medico.Nome),
                new Claim("crm", medico.CRM),
                new Claim("role", "medico")
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
