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

        var tokenHandler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };

        var key = Convert.FromHexString(_configuration["JWT_SECRET"]!);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

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
