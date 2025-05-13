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
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Buscar(
    [FromQuery] string? nome,
    [FromQuery] string? especialidade,
    [FromQuery] string? cidade,
    [FromQuery] string? uf)
    {
        var filtro = new FiltroMedico
        {
            Nome = nome,
            Especialidade = especialidade,
            Cidade = cidade,
            UF = uf
        };

        var medicos = await _medicoService.BuscarAsync(filtro);

        var resultado = medicos.Select(m => new MedicoDto
        {
            Id = m.Id,
            Nome = m.Nome,
            CRM = m.CRM,
            Especialidade = m.Especialidade,
            Cidade = m.Cidade,
            UF = m.UF
        });

        return Ok(resultado);
    }

}
