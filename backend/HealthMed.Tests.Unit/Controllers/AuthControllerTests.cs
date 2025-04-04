using Xunit;
using Moq;
using HealthMed.Api.Controllers;
using HealthMed.Application.DTOs;
using HealthMed.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HealthMed.Tests.Unit.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact(DisplayName = "Up deve retornar 200 com status ok")]
    public void Up_DeveRetornarOk()
    {

        var resultado = _controller.Up() as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Equal("ok", resultado.Value!.GetType().GetProperty("status")!.GetValue(resultado.Value));
    }

    [Fact(DisplayName = "Login deve retornar token quando credenciais estão corretas")]
    public async Task LoginMedico_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        var dto = new LoginMedicoDto
        {
            CRM = "12345",
            Senha = "senha123"
        };

        var token = "token.jwt.simulado";

        _authServiceMock.Setup(s => s.AutenticarMedicoAsync(dto.CRM, dto.Senha))
                        .ReturnsAsync(token);

        var resultado = await _controller.LoginMedico(dto) as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Equal(token, resultado.Value!.GetType().GetProperty("token")!.GetValue(resultado.Value));
    }

    [Fact(DisplayName = "Login deve retornar 401 quando credenciais são inválidas")]
    public async Task LoginMedico_DeveRetornarUnauthorized_QuandoCredenciaisInvalidas()
    {
        var dto = new LoginMedicoDto
        {
            CRM = "12345",
            Senha = "senha_errada"
        };

        _authServiceMock.Setup(s => s.AutenticarMedicoAsync(dto.CRM, dto.Senha))
                        .ReturnsAsync((string?)null);

        var resultado = await _controller.LoginMedico(dto) as UnauthorizedObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(401, resultado!.StatusCode);
    }
}
