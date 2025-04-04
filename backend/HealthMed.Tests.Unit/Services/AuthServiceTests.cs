using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HealthMed.Application.Services;
using HealthMed.Infrastructure.Auth;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Interfaces;

namespace HealthMed.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<IMedicoRepository> _repoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _repoMock = new Mock<IMedicoRepository>();
        _configMock = new Mock<IConfiguration>();

        _configMock.Setup(c => c["JWT_SECRET"])
            .Returns("00112233445566778899AABBCCDDEEFF00112233445566778899AABBCCDDEEFF");

        _service = new AuthService(_repoMock.Object, _configMock.Object);
    }

    [Fact(DisplayName = "AutenticarMedicoAsync deve retornar token se CRM e senha forem válidos")]
    public async Task AutenticarMedico_DeveRetornarToken()
    {
        var senha = "minhasenha";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        var medico = new Medico(Guid.NewGuid(), "Dr. Bruno", "CRM123", "Clínico", senhaHash, "BH", "MG");

        _repoMock.Setup(r => r.ObterPorCRMAsync("CRM123")).ReturnsAsync(medico);

        var token = await _service.AutenticarMedicoAsync("CRM123", senha);

        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact(DisplayName = "AutenticarMedicoAsync deve retornar null se senha for inválida")]
    public async Task AutenticarMedico_DeveRetornarNull_SeSenhaIncorreta()
    {
        var medico = new Medico(Guid.NewGuid(), "Dra. Ana", "CRM456", "Dermato", BCrypt.Net.BCrypt.HashPassword("correta"), "SP", "SP");

        _repoMock.Setup(r => r.ObterPorCRMAsync("CRM456")).ReturnsAsync(medico);

        var token = await _service.AutenticarMedicoAsync("CRM456", "errada");

        Assert.Null(token);
    }

    [Fact(DisplayName = "AutenticarMedicoAsync deve retornar null se médico não for encontrado")]
    public async Task AutenticarMedico_DeveRetornarNull_SeMedicoInexistente()
    {
        _repoMock.Setup(r => r.ObterPorCRMAsync("CRM999")).ReturnsAsync((Medico?)null);

        var token = await _service.AutenticarMedicoAsync("CRM999", "qualquer");

        Assert.Null(token);
    }

}
