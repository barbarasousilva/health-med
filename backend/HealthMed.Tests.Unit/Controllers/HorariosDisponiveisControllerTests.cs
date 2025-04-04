using Xunit;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthMed.Api.Controllers;
using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthMed.Tests.Unit.Controllers;

public class HorariosDisponiveisControllerTests
{
    private readonly Mock<IHorarioDisponivelService> _serviceMock;
    private readonly HorariosDisponiveisController _controller;
    private readonly Guid _medicoId;

    public HorariosDisponiveisControllerTests()
    {
        _serviceMock = new Mock<IHorarioDisponivelService>();

        _controller = new HorariosDisponiveisController(_serviceMock.Object);

        _medicoId = Guid.NewGuid();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("id", _medicoId.ToString()),
            new Claim("crm", "123456"),
            new Claim("role", "medico"),
            new Claim(ClaimTypes.Name, "Dr. Teste")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact(DisplayName = "Up deve retornar 200 com dados do médico")]
    public void Up_DeveRetornarOk()
    {
        var resultado = _controller.Up() as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Contains("ok", resultado.Value!.ToString());
        Assert.Contains("medicoId", resultado.Value!.ToString());
    }

    [Fact(DisplayName = "Listar deve retornar lista de horários do médico")]
    public async Task Listar_DeveRetornarListaHorarios()
    {
        var horarios = new List<HorarioDisponivel>
        {
            new HorarioDisponivel(Guid.NewGuid(), _medicoId, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), StatusHorario.Disponivel)
        };

        _serviceMock.Setup(s => s.ListarPorMedicoAsync(_medicoId)).ReturnsAsync(horarios);

        var resultado = await _controller.Listar() as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.IsAssignableFrom<IEnumerable<HorarioDisponivel>>(resultado.Value);
    }

    [Fact(DisplayName = "Cadastrar deve retornar 201 Created com id")]
    public async Task Cadastrar_DeveRetornarCreated()
    {
        var dto = new CadastrarHorarioDto
        {
            DataHora = DateTime.UtcNow.AddHours(1),
            DataHoraFim = DateTime.UtcNow.AddHours(2),
            Status = StatusHorario.Disponivel
        };

        var novoId = Guid.NewGuid();

        _serviceMock.Setup(s =>
            s.AdicionarAsync(_medicoId, dto.DataHora, dto.DataHoraFim, dto.Status)
        ).ReturnsAsync(novoId);

        var resultado = await _controller.Cadastrar(dto) as CreatedAtActionResult;

        Assert.NotNull(resultado);
        Assert.Equal(201, resultado!.StatusCode);
        Assert.Equal(nameof(_controller.Listar), resultado.ActionName);
        Assert.Equal(novoId, resultado.RouteValues!["id"]);
    }

    [Fact(DisplayName = "Atualizar deve retornar 204 NoContent")]
    public async Task Atualizar_DeveRetornarNoContent()
    {
        var id = Guid.NewGuid();
        var dto = new EditarHorarioDto
        {
            DataHora = DateTime.UtcNow.AddHours(1),
            DataHoraFim = DateTime.UtcNow.AddHours(2),
            Status = StatusHorario.Disponivel
        };

        _serviceMock.Setup(s =>
            s.AtualizarAsync(id, _medicoId, dto.DataHora, dto.DataHoraFim, dto.Status)
        ).Returns(Task.CompletedTask);

        var resultado = await _controller.Atualizar(id, dto) as NoContentResult;

        Assert.NotNull(resultado);
        Assert.Equal(204, resultado!.StatusCode);
    }

    [Fact(DisplayName = "Remover deve retornar 204 NoContent")]
    public async Task Remover_DeveRetornarNoContent()
    {
        var id = Guid.NewGuid();

        _serviceMock.Setup(s =>
            s.RemoverAsync(id, _medicoId)
        ).Returns(Task.CompletedTask);

        var resultado = await _controller.Remover(id) as NoContentResult;

        Assert.NotNull(resultado);
        Assert.Equal(204, resultado!.StatusCode);
    }

    [Fact(DisplayName = "AbrirAgenda deve retornar 200 OK com mensagem")]
    public async Task AbrirAgenda_DeveRetornarOk()
    {
        var dto = new AbrirAgendaDto
        {
            Data = DateTime.Today.AddDays(1),
            Duracao = DuracaoConsulta.TrintaMinutos
        };

        _serviceMock.Setup(s =>
            s.AbrirAgendaAsync(_medicoId, dto.Data, dto.Duracao.ToTimeSpan())
        ).Returns(Task.CompletedTask);

        var resultado = await _controller.AbrirAgenda(dto) as OkObjectResult;

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.StatusCode);
        Assert.Contains("Agenda aberta", resultado.Value!.ToString());
    }
}
