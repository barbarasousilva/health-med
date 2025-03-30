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

        var tokenHandler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };
        var key = Convert.FromHexString(_configuration["JWT_SECRET"]!);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("id", medico.Id.ToString()),
            new Claim(ClaimTypes.Name, medico.Nome),
            new Claim("crm", medico.CRM),
            new Claim(ClaimTypes.Role, "medico")
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
}
