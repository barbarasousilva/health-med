using Xunit;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using HealthMed.API.Controllers;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;
using HealthMed.Application.DTOs;
using Domain.Entities;

namespace HealthMed.Tests.Unit.Controllers;

public class ConsultaControllerTests
{
    private readonly Mock<IConsultaService> _serviceMock;
    private readonly ConsultaController _controller;
    private readonly Guid _usuarioId;

    public ConsultaControllerTests()
    {
        _serviceMock = new Mock<IConsultaService>();
        _controller = new ConsultaController(_serviceMock.Object);
        _usuarioId = Guid.NewGuid();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, _usuarioId.ToString()),
            new Claim(ClaimTypes.Role, "paciente"),
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact(DisplayName = "AgendarConsulta deve retornar 200 OK quando agendamento for bem-sucedido")]
    public async Task AgendarConsulta_DeveRetornarOk()
    {
        var dto = new AgendarConsultaDto
        {
            IdMedico = Guid.NewGuid(),
            IdHorarioDisponivel = Guid.NewGuid()
        };

        _serviceMock.Setup(s =>
            s.AgendarConsultaAsync(_usuarioId, dto.IdMedico, dto.IdHorarioDisponivel)
        ).ReturnsAsync(Guid.NewGuid());

        var resultado = await _controller.AgendarConsulta(dto) as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Contains("Consulta agendada", resultado.Value!.ToString());
    }

    [Fact(DisplayName = "CancelarConsulta deve retornar 200 OK")]
    public async Task CancelarConsulta_DeveRetornarOk()
    {
        var id = Guid.NewGuid();
        var dto = new CancelarConsultaDto { Justificativa = "Preciso remarcar" };

        _serviceMock.Setup(s =>
            s.CancelarConsultaAsync(id, dto.Justificativa)
        ).Returns(Task.CompletedTask);

        var resultado = await _controller.CancelarConsulta(id, dto) as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Contains("cancelada", resultado.Value!.ToString());
    }

    [Fact(DisplayName = "ListarConsultasPendentes deve retornar 200 OK com lista")]
    public async Task ListarConsultasPendentes_DeveRetornarOk()
    {
        var medicoId = Guid.NewGuid();
        var pendenteMock = new List<Consulta>
        {
            new Consulta
            {
                Id = Guid.NewGuid(),
                IdMedico = medicoId,
                IdPaciente = Guid.NewGuid(),
                IdHorarioDisponivel = Guid.NewGuid(),
                Status = StatusConsulta.Pendente,
                DataAgendamento = DateTime.UtcNow,
                JustificativaCancelamento = ""
            }
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("id", medicoId.ToString()),
            new Claim(ClaimTypes.Role, "medico")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _serviceMock.Setup(s => s.ListarPorStatusAsync(medicoId, StatusConsulta.Pendente))
            .ReturnsAsync(pendenteMock);

        var resultado = await _controller.ListarConsultasPendentes() as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        var retorno = Assert.IsType<List<ConsultaDto>>(resultado!.Value);
        Assert.Single(retorno);
        Assert.Equal(StatusConsulta.Pendente, retorno[0].Status);
        Assert.Equal(pendenteMock[0].Id, retorno[0].Id);
    }

    [Fact(DisplayName = "AceitarConsulta deve retornar 200 OK")]
    public async Task AceitarConsulta_DeveRetornarOk()
    {
        var idConsulta = Guid.NewGuid();

        _serviceMock.Setup(s => s.AceitarConsultaAsync(idConsulta))
            .Returns(Task.CompletedTask);

        var resultado = await _controller.AceitarConsulta(idConsulta) as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Contains("aceita", resultado.Value!.ToString());
    }

    [Fact(DisplayName = "RecusarConsulta deve retornar 200 OK")]
    public async Task RecusarConsulta_DeveRetornarOk()
    {
        var idConsulta = Guid.NewGuid();

        _serviceMock.Setup(s => s.RecusarConsultaAsync(idConsulta))
            .Returns(Task.CompletedTask);

        var resultado = await _controller.RecusarConsulta(idConsulta) as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Contains("recusada", resultado.Value!.ToString());
    }
}
