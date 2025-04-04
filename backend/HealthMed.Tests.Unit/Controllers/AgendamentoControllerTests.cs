using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthMed.Api.Controllers;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;

namespace HealthMed.Tests.Unit.Controllers;

public class AgendamentosControllerTests
{
    private readonly Mock<IAgendamentoConsultaService> _serviceMock;
    private readonly AgendamentosController _controller;

    public AgendamentosControllerTests()
    {
        _serviceMock = new Mock<IAgendamentoConsultaService>();
        _controller = new AgendamentosController(_serviceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("id", Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "paciente")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact(DisplayName = "BuscarHorariosDisponiveis deve retornar 200 com resultados")]
    public async Task BuscarHorariosDisponiveis_DeveRetornarOkComResultados()
    {
        var resultadoEsperado = new List<object>
        {
            new
            {
                Medico = "Dr. Teste",
                DataHora = DateTime.UtcNow.AddHours(1),
                Especialidade = "Cardiologia"
            }
        };

        _serviceMock.Setup(s => s.BuscarHorariosAsync(null, null, null, null))
                    .ReturnsAsync(resultadoEsperado);

        var resultado = await _controller.BuscarHorariosDisponiveis(null, null, null, null) as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Equal(resultadoEsperado, resultado.Value);
    }

    [Fact(DisplayName = "BuscarHorariosDisponiveis deve aceitar filtros")]
    public async Task BuscarHorariosDisponiveis_DeveAceitarFiltros()
    {
        var data = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var status = StatusHorario.Disponivel;
        var especialidade = "Pediatria";
        var medicoId = Guid.NewGuid();

        var resultadoEsperado = new List<object>
        {
            new
            {
                Medico = "Dra. Pediatra",
                DataHora = DateTime.UtcNow.AddHours(2),
                Especialidade = "Pediatria"
            }
        };

        _serviceMock.Setup(s => s.BuscarHorariosAsync(data, status, especialidade, medicoId))
                    .ReturnsAsync(resultadoEsperado);

        var resultado = await _controller.BuscarHorariosDisponiveis(data, status, especialidade, medicoId) as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Equal(resultadoEsperado, resultado.Value);
    }
}
