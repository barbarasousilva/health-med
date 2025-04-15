using Xunit;
using Moq;
using HealthMed.Api.Controllers;
using HealthMed.Application.DTOs;
using HealthMed.Domain.Entities;
using HealthMed.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HealthMed.Tests.Unit.Controllers;

public class MedicoControllerTests
{
    private readonly Mock<IMedicoService> _serviceMock;
    private readonly MedicoController _controller;

    public MedicoControllerTests()
    {
        _serviceMock = new Mock<IMedicoService>();
        _controller = new MedicoController(_serviceMock.Object);
    }

    [Fact(DisplayName = "Registrar deve retornar Ok quando médico for registrado com sucesso")]
    public async Task Registrar_DeveRetornarOk_QuandoMedicoValido()
    {

        var dto = new RegistrarMedicoDto
        {
            Nome = "Dr. House",
            CRM = "12345",
            Especialidade = "Clínico Geral",
            Senha = "senhaSegura",
            Cidade = "Princeton",
            UF = "NJ"
        };

        var idSimulado = Guid.NewGuid();
        _serviceMock.Setup(s => s.RegistrarMedicoAsync(It.IsAny<Medico>()))
                    .ReturnsAsync(idSimulado);

        var resultado = await _controller.Registrar(dto) as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Equal(idSimulado, resultado.Value!.GetType().GetProperty("id")!.GetValue(resultado.Value));
    }

    [Fact(DisplayName = "Registrar deve retornar Conflict quando CRM já existir")]
    public async Task Registrar_DeveRetornarConflict_QuandoCRMExistente()
    {
        var dto = new RegistrarMedicoDto
        {
            Nome = "Dra. House",
            CRM = "12345",
            Especialidade = "Clínico Geral",
            Senha = "senhaSegura",
            Cidade = "Princeton",
            UF = "NJ"
        };

        _serviceMock.Setup(s => s.RegistrarMedicoAsync(It.IsAny<Medico>()))
                    .ThrowsAsync(new InvalidOperationException("CRM já cadastrado."));

        var resultado = await _controller.Registrar(dto) as ConflictObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(409, resultado!.StatusCode);
        Assert.Contains("CRM já cadastrado.", resultado.Value!.ToString());
    }
}
