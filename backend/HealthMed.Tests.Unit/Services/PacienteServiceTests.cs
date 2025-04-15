using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Interfaces;
using HealthMed.Application.Services;
using HealthMed.Infrastructure.Auth;
using Microsoft.Extensions.Configuration;

namespace HealthMed.Tests.Unit.Services;

public class PacienteServiceTests
{
    private readonly Mock<IPacienteRepository> _repoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly PacienteService _service;

    public PacienteServiceTests()
    {
        _repoMock = new Mock<IPacienteRepository>();
        _configMock = new Mock<IConfiguration>();

        _configMock.Setup(c => c["JWT_SECRET"]).Returns("00112233445566778899AABBCCDDEEFF00112233445566778899AABBCCDDEEFF");

        _service = new PacienteService(_repoMock.Object, _configMock.Object);
    }

    [Fact(DisplayName = "RegistrarPacienteAsync deve adicionar paciente se CPF/e-mail forem únicos")]
    public async Task RegistrarPaciente_DeveCadastrarPacienteValido()
    {
        var paciente = new Paciente(Guid.NewGuid(), "Maria", "maria@exemplo.com", "12345678900", "123456");

        _repoMock.Setup(r => r.ObterPorEmailOuCpfAsync(It.IsAny<string>(), It.IsAny<string>()))
                 .ReturnsAsync((Paciente?)null);

        await _service.RegistrarPacienteAsync(paciente, "123456");

        _repoMock.Verify(r => r.AdicionarAsync(It.Is<Paciente>(p =>
            !string.IsNullOrEmpty(p.SenhaHash) &&
            p.SenhaHash != "123456"
        )), Times.Once);

    }

    [Fact(DisplayName = "RegistrarPacienteAsync deve lançar exceção se paciente já existir")]
    public async Task RegistrarPaciente_DeveFalhar_SeJaExistir()
    {
        var existente = new Paciente(Guid.NewGuid(), "Maria", "maria@exemplo.com", "12345678900", "senha");
        _repoMock.Setup(r => r.ObterPorEmailOuCpfAsync("maria@exemplo.com", "12345678900"))
                 .ReturnsAsync(existente);

        var novo = new Paciente(Guid.NewGuid(), "Maria", "maria@exemplo.com", "12345678900", "novaSenha");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.RegistrarPacienteAsync(novo, "novaSenha"));
    }

    [Fact(DisplayName = "AutenticarPacienteAsync deve retornar token se senha for válida")]
    public async Task AutenticarPaciente_DeveRetornarToken()
    {
        var senha = "minhaSenha";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

        var paciente = new Paciente(Guid.NewGuid(), "João", "joao@email.com", "12345678900", senhaHash);

        _repoMock.Setup(r => r.ObterPorCpfOuEmailAsync("joao@email.com"))
                 .ReturnsAsync(paciente);

        var token = await _service.AutenticarPacienteAsync("joao@email.com", senha);

        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact(DisplayName = "AutenticarPacienteAsync deve retornar null se senha for inválida")]
    public async Task AutenticarPaciente_DeveRetornarNull_SeSenhaIncorreta()
    {
        var paciente = new Paciente(Guid.NewGuid(), "João", "joao@email.com", "12345678900",
            BCrypt.Net.BCrypt.HashPassword("correta"));

        _repoMock.Setup(r => r.ObterPorCpfOuEmailAsync("joao@email.com"))
                 .ReturnsAsync(paciente);

        var token = await _service.AutenticarPacienteAsync("joao@email.com", "senhaErrada");

        Assert.Null(token);
    }

    [Fact(DisplayName = "AutenticarPacienteAsync deve retornar null se paciente não existir")]
    public async Task AutenticarPaciente_DeveRetornarNull_SeNaoExistir()
    {
        _repoMock.Setup(r => r.ObterPorCpfOuEmailAsync("nao@existe.com"))
                 .ReturnsAsync((Paciente?)null);

        var token = await _service.AutenticarPacienteAsync("nao@existe.com", "123");

        Assert.Null(token);
    }
}
