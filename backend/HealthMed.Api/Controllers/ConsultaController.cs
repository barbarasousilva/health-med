using HealthMed.Api.Helpers;
using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthMed.API.Controllers;

[ApiController]
[Route("api/consultas")]
public class ConsultaController : ControllerBase
{
    private readonly IConsultaService _consultaService;

    public ConsultaController(IConsultaService consultaService)
    {
        _consultaService = consultaService;
    }


    [HttpPost]
    [Authorize(Roles = "paciente")]
    public async Task<IActionResult> AgendarConsulta([FromBody] AgendarConsultaDto dto)
    {
        var idPaciente = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idPaciente == null) return Unauthorized();

        var consultaId = await _consultaService.AgendarConsultaAsync(Guid.Parse(idPaciente), dto.IdMedico, dto.IdHorarioDisponivel);

        return Ok(new
        {
            mensagem = "Consulta agendada com sucesso.",
            id = consultaId
        });
    }

    [HttpPut("{id}/cancelar")]
    [Authorize(Roles = "paciente")]
    public async Task<IActionResult> CancelarConsulta(Guid id, [FromBody] CancelarConsultaDto dto)
    {
        await _consultaService.CancelarConsultaAsync(id, dto.Justificativa);
        return Ok(new { mensagem = "Consulta cancelada com sucesso." });
    }

    [HttpGet("pendentes")]
    [Authorize(Roles = "medico")]
    public async Task<IActionResult> ListarConsultasPendentes()
    {
        var medicoId = User.GetMedicoId();
        var consultas = await _consultaService.ListarPorStatusAsync(medicoId, StatusConsulta.Pendente);

        var consultasDto = consultas.Select(c => new ConsultaDto
        {
            Id = c.Id,
            MedicoId = c.IdMedico,
            PacienteId = c.IdPaciente,
            HorarioDisponivelId = c.IdHorarioDisponivel,
            Status = c.Status,
            DataAgendamento = c.DataAgendamento,
            JustificativaCancelamento = c.JustificativaCancelamento ?? string.Empty
        }).ToList();

        return Ok(consultasDto);
    }

    [HttpPut("{id}/aceitar")]
    [Authorize(Roles = "medico")]
    public async Task<IActionResult> AceitarConsulta(Guid id)
    {
        await _consultaService.AceitarConsultaAsync(id);
        return Ok(new { mensagem = "Consulta aceita." });
    }

    [HttpPut("{id}/recusar")]
    [Authorize(Roles = "medico")]
    public async Task<IActionResult> RecusarConsulta(Guid id)
    {
        await _consultaService.RecusarConsultaAsync(id);
        return Ok(new { mensagem = "Consulta recusada." });
    }
}
