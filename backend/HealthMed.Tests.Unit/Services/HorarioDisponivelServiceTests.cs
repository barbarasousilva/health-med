using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;
using HealthMed.Application.Services;

namespace HealthMed.Tests.Unit.Services;

public class HorarioDisponivelServiceTests
{
    private readonly Mock<IHorarioDisponivelRepository> _repoMock;
    private readonly HorarioDisponivelService _service;

    public HorarioDisponivelServiceTests()
    {
        _repoMock = new Mock<IHorarioDisponivelRepository>();
        _service = new HorarioDisponivelService(_repoMock.Object);
    }

    [Fact(DisplayName = "AdicionarAsync deve adicionar horário válido com sucesso")]
    public async Task AdicionarAsync_DeveCriarHorarioValido()
    {
        var medicoId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddHours(1);
        var dataFim = dataInicio.AddMinutes(30);
        var status = StatusHorario.Disponivel;

        var id = await _service.AdicionarAsync(medicoId, dataInicio, dataFim, status);

        _repoMock.Verify(r =>
            r.AdicionarAsync(It.Is<HorarioDisponivel>(h =>
                h.MedicoId == medicoId &&
                h.DataHora == dataInicio &&
                h.DataHoraFim == dataFim &&
                h.Status == status
            )), Times.Once);

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact(DisplayName = "AdicionarAsync deve lançar exceção se dataHora >= dataHoraFim")]
    public async Task AdicionarAsync_DeveFalhar_QuandoDataInvalida()
    {
        var medicoId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddHours(2);
        var dataFim = DateTime.UtcNow.AddHours(1);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AdicionarAsync(medicoId, dataInicio, dataFim, StatusHorario.Disponivel));
    }

    [Fact(DisplayName = "AdicionarAsync deve lançar exceção se dataHora for no passado")]
    public async Task AdicionarAsync_DeveFalhar_QuandoDataPassada()
    {
        var medicoId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddMinutes(-10);
        var dataFim = DateTime.UtcNow.AddMinutes(10);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AdicionarAsync(medicoId, dataInicio, dataFim, StatusHorario.Disponivel));
    }

    [Fact(DisplayName = "AtualizarAsync deve atualizar quando dados e médico são válidos")]
    public async Task AtualizarAsync_DeveAtualizarComSucesso()
    {
        var id = Guid.NewGuid();
        var medicoId = Guid.NewGuid();
        var horarioExistente = new HorarioDisponivel(
            id, medicoId, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), StatusHorario.Disponivel
        );

        _repoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(horarioExistente);

        var novaInicio = DateTime.UtcNow.AddHours(3);
        var novaFim = DateTime.UtcNow.AddHours(4);

        await _service.AtualizarAsync(id, medicoId, novaInicio, novaFim, StatusHorario.Ocupado);

        _repoMock.Verify(r => r.AtualizarAsync(It.Is<HorarioDisponivel>(h =>
            h.DataHora == novaInicio &&
            h.DataHoraFim == novaFim &&
            h.Status == StatusHorario.Ocupado
        )), Times.Once);
    }

    [Fact(DisplayName = "AtualizarAsync deve lançar exceção se médico não for o dono")]
    public async Task AtualizarAsync_DeveFalhar_SeMedicoNaoForDono()
    {
        var id = Guid.NewGuid();
        var donoId = Guid.NewGuid();
        var outroId = Guid.NewGuid();

        var horario = new HorarioDisponivel(
            id, donoId, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), StatusHorario.Disponivel
        );

        _repoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(horario);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.AtualizarAsync(id, outroId, DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(4), StatusHorario.Disponivel));
    }

    [Fact(DisplayName = "AtualizarAsync deve lançar exceção se datas forem inválidas")]
    public async Task AtualizarAsync_DeveFalhar_SeDatasInvalidas()
    {
        var id = Guid.NewGuid();
        var medicoId = Guid.NewGuid();
        var horario = new HorarioDisponivel(
            id, medicoId, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), StatusHorario.Disponivel
        );

        _repoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(horario);

        var novaInicio = DateTime.UtcNow.AddHours(4);
        var novaFim = DateTime.UtcNow.AddHours(3);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AtualizarAsync(id, medicoId, novaInicio, novaFim, StatusHorario.Disponivel));
    }

    [Fact(DisplayName = "RemoverAsync deve remover horário se for do médico")]
    public async Task RemoverAsync_DeveRemover_SeForDoMedico()
    {
        var id = Guid.NewGuid();
        var medicoId = Guid.NewGuid();
        var horario = new HorarioDisponivel(
            id, medicoId, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), StatusHorario.Disponivel
        );

        _repoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(horario);

        await _service.RemoverAsync(id, medicoId);

        _repoMock.Verify(r => r.RemoverAsync(id), Times.Once);
    }

    [Fact(DisplayName = "RemoverAsync deve lançar exceção se médico não for o dono")]
    public async Task RemoverAsync_DeveFalhar_SeOutroMedico()
    {
        var id = Guid.NewGuid();
        var dono = Guid.NewGuid();
        var outro = Guid.NewGuid();

        var horario = new HorarioDisponivel(
            id, dono, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), StatusHorario.Disponivel
        );

        _repoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(horario);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.RemoverAsync(id, outro));
    }

    [Fact(DisplayName = "AbrirAgendaAsync deve gerar horários corretamente")]
    public async Task AbrirAgendaAsync_DeveGerarHorariosNovos()
    {
        var medicoId = Guid.NewGuid();
        var data = DateTime.Today.AddDays(1); // futuro
        var duracao = TimeSpan.FromMinutes(30);

        _repoMock.Setup(r => r.ListarPorMedicoAsync(medicoId)).ReturnsAsync(new List<HorarioDisponivel>());

        await _service.AbrirAgendaAsync(medicoId, data, duracao);

        _repoMock.Verify(r => r.AdicionarAsync(It.IsAny<HorarioDisponivel>()), Times.Exactly(20));
    }


}
