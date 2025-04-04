using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;
using HealthMed.Application.Services;

namespace HealthMed.Tests.Unit.Services;

public class AgendamentoConsultaServiceTests
{
    private readonly Mock<IHorarioDisponivelRepository> _repoMock;
    private readonly AgendamentoConsultaService _service;

    public AgendamentoConsultaServiceTests()
    {
        _repoMock = new Mock<IHorarioDisponivelRepository>();
        _service = new AgendamentoConsultaService(_repoMock.Object);
    }

    [Fact(DisplayName = "BuscarHorariosAsync deve chamar repositório com todos os filtros")]
    public async Task BuscarHorarios_DeveChamarRepositorio()
    {
        var data = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var status = StatusHorario.Disponivel;
        var especialidade = "Cardiologia";
        var medicoId = Guid.NewGuid();

        var resultadoEsperado = new List<object> { new { NomeMedico = "Dra. Ana", Especialidade = "Cardiologia" } };

        _repoMock.Setup(r => r.BuscarHorariosAsync(data, status, especialidade, medicoId))
                 .ReturnsAsync(resultadoEsperado);

        var resultado = await _service.BuscarHorariosAsync(data, status, especialidade, medicoId);

        Assert.Single(resultado);
        _repoMock.Verify(r => r.BuscarHorariosAsync(data, status, especialidade, medicoId), Times.Once);
    }

    [Fact(DisplayName = "BuscarHorariosAsync deve funcionar mesmo sem filtros")]
    public async Task BuscarHorarios_DeveFuncionarSemFiltros()
    {
        var resultadoEsperado = new List<object>
    {
        new { NomeMedico = "Dr. Bruno", Especialidade = "Ortopedia" },
        new { NomeMedico = "Dra. Clara", Especialidade = "Dermato" }
    };

        _repoMock.Setup(r => r.BuscarHorariosAsync(null, null, null, null))
                 .ReturnsAsync(resultadoEsperado);

        var resultado = await _service.BuscarHorariosAsync(null, null, null, null);

        Assert.Equal(2, resultado.Count());
        _repoMock.Verify(r => r.BuscarHorariosAsync(null, null, null, null), Times.Once);
    }


}
