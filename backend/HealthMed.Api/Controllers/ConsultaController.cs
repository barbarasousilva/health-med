using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthMed.API.Controllers;

[ApiController]
[Route("consultas")]
public class ConsultaController : ControllerBase
{
    private readonly ConsultaService _consultaService;

    public ConsultaController(ConsultaService consultaService)
    {
        _consultaService = consultaService;
    }

    [HttpPost]
    [Authorize(Roles = "paciente")]
    public async Task<IActionResult> AgendarConsulta([FromBody] AgendarConsultaDto dto)
    {
        var idPaciente = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idPaciente == null) return Unauthorized();

        await _consultaService.AgendarConsultaAsync(Guid.Parse(idPaciente), dto.IdMedico, dto.IdHorarioDisponivel);
        return Ok(new { mensagem = "Consulta agendada com sucesso." });
    }

    [HttpPut("{id}/cancelar")]
    [Authorize(Roles = "paciente")]
    public async Task<IActionResult> CancelarConsulta(Guid id, [FromBody] CancelarConsultaDto dto)
    {
        await _consultaService.CancelarConsultaAsync(id, dto.Justificativa);
        return Ok(new { mensagem = "Consulta cancelada com sucesso." });
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
