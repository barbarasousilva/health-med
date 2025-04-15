using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers;

[ApiController]
[Route("api/medicos")]
public class MedicoController : ControllerBase
{
    private readonly IMedicoService _medicoService;

    public MedicoController(IMedicoService medicoService)
    {
        _medicoService = medicoService;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarMedicoDto dto)
    {
        var medico = new Medico(
            Guid.NewGuid(),
            dto.Nome,
            dto.CRM,
            dto.Especialidade,
            BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            dto.Cidade,
            dto.UF
        );

        try
        {
            var id = await _medicoService.RegistrarMedicoAsync(medico);
            return Ok(new { id, mensagem = "Médico registrado com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }
}
