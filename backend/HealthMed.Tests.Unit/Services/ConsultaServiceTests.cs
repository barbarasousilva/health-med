using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Domain.Entities;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;
using HealthMed.Application.Services;

namespace HealthMed.Tests.Unit.Services;

public class ConsultaServiceTests
{
    private readonly Mock<IConsultaRepository> _repoMock;
    private readonly ConsultaService _service;

    public ConsultaServiceTests()
    {
        _repoMock = new Mock<IConsultaRepository>();
        _service = new ConsultaService(_repoMock.Object);
    }

    [Fact(DisplayName = "AgendarConsultaAsync deve adicionar consulta se horário estiver disponível")]
    public async Task AgendarConsulta_DeveFuncionar()
    {
        var idPaciente = Guid.NewGuid();
        var idMedico = Guid.NewGuid();
        var idHorario = Guid.NewGuid();

        _repoMock.Setup(r => r.HorarioJaAgendadoAsync(idHorario))
                 .ReturnsAsync(false);

        await _service.AgendarConsultaAsync(idPaciente, idMedico, idHorario);

        _repoMock.Verify(r => r.AdicionarAsync(It.Is<Consulta>(c =>
            c.IdPaciente == idPaciente &&
            c.IdMedico == idMedico &&
            c.IdHorarioDisponivel == idHorario &&
            c.Status == StatusConsulta.Pendente
        )), Times.Once);
    }

    [Fact(DisplayName = "AgendarConsultaAsync deve lançar exceção se horário já estiver ocupado")]
    public async Task AgendarConsulta_DeveFalhar_SeHorarioOcupado()
    {
        var idHorario = Guid.NewGuid();

        _repoMock.Setup(r => r.HorarioJaAgendadoAsync(idHorario))
                 .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AgendarConsultaAsync(Guid.NewGuid(), Guid.NewGuid(), idHorario));
    }

    [Fact(DisplayName = "CancelarConsultaAsync deve atualizar status e justificativa")]
    public async Task CancelarConsulta_DeveAtualizarStatus()
    {
        var idConsulta = Guid.NewGuid();
        var justificativa = "Paciente teve imprevisto";

        await _service.CancelarConsultaAsync(idConsulta, justificativa);

        _repoMock.Verify(r => r.AtualizarStatusAsync(
            idConsulta,
            StatusConsulta.Cancelada.ToString(),
            justificativa,
            null,
            It.IsAny<DateTime>()
        ), Times.Once);
    }

    [Fact(DisplayName = "AceitarConsultaAsync deve atualizar status e data de resposta")]
    public async Task AceitarConsulta_DeveAtualizarStatus()
    {
        var idConsulta = Guid.NewGuid();

        await _service.AceitarConsultaAsync(idConsulta);

        _repoMock.Verify(r => r.AtualizarStatusAsync(
            idConsulta,
            StatusConsulta.Aceita.ToString(),
            null,
            It.IsAny<DateTime>(),
            null
        ), Times.Once);
    }

    [Fact(DisplayName = "RecusarConsultaAsync deve atualizar status e data de resposta")]
    public async Task RecusarConsulta_DeveAtualizarStatus()
    {
        var idConsulta = Guid.NewGuid();

        await _service.RecusarConsultaAsync(idConsulta);

        _repoMock.Verify(r => r.AtualizarStatusAsync(
            idConsulta,
            StatusConsulta.Recusada.ToString(),
            null,
            It.IsAny<DateTime>(),
            null
        ), Times.Once);
    }



}
