using Xunit;
using Moq;
using HealthMed.Api.Controllers;
using HealthMed.Application.Services;
using HealthMed.Application.DTOs;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HealthMed.Tests.Unit.Controllers;

public class PacienteControllerTests
{
    private readonly Mock<IPacienteService> _serviceMock;
    private readonly PacienteController _controller;

    public PacienteControllerTests()
    {
        _serviceMock = new Mock<IPacienteService>();
        _controller = new PacienteController(_serviceMock.Object);
    }

    [Fact(DisplayName = "Login deve retornar token válido quando credenciais são corretas")]
    public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas()
    {

        var dto = new LoginPacienteDto { CpfOuEmail = "teste@teste.com", Senha = "123456" };
        var tokenSimulado = "token.jwt.simulado";
        _serviceMock.Setup(s => s.AutenticarPacienteAsync(dto.CpfOuEmail, dto.Senha))
                    .ReturnsAsync(tokenSimulado);

        var resultado = await _controller.Login(dto) as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Equal(tokenSimulado, resultado.Value!.GetType().GetProperty("token")!.GetValue(resultado.Value));
    }

    [Fact(DisplayName = "Login deve retornar Unauthorized quando credenciais são inválidas")]
    public async Task Login_DeveRetornarUnauthorized_QuandoCredenciaisInvalidas()
    {

        var dto = new LoginPacienteDto { CpfOuEmail = "teste@teste.com", Senha = "errada" };
        _serviceMock.Setup(s => s.AutenticarPacienteAsync(dto.CpfOuEmail, dto.Senha))
                    .ReturnsAsync((string?)null);


        var resultado = await _controller.Login(dto) as UnauthorizedObjectResult;


        Assert.NotNull(resultado);
        Assert.Equal(401, resultado!.StatusCode);
    }

    [Fact(DisplayName = "Registrar deve retornar Created quando paciente é válido")]
    public async Task Registrar_DeveRetornarCreated_QuandoPacienteValido()
    {

        var dto = new RegistrarPacienteDto
        {
            Nome = "Paciente Teste",
            Email = "paciente@teste.com",
            Cpf = "12345678900",
            Senha = "123456"
        };

        var pacienteId = Guid.NewGuid();
        _serviceMock.Setup(s => s.RegistrarPacienteAsync(It.IsAny<Paciente>(), dto.Senha))
                    .ReturnsAsync(pacienteId);


        var resultado = await _controller.Registrar(dto) as CreatedAtActionResult;


        Assert.NotNull(resultado);
        Assert.Equal(201, resultado!.StatusCode);
        Assert.Equal(nameof(_controller.Registrar), resultado.ActionName);
        Assert.Equal(pacienteId, resultado.RouteValues!["id"]);
    }
}
