using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers;

[ApiController]
[Route("api/auth/paciente")]
public class PacienteController : ControllerBase
{
    private readonly IPacienteService _service;

    public PacienteController(IPacienteService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginPacienteDto dto)
    {
        var token = await _service.AutenticarPacienteAsync(dto.CpfOuEmail, dto.Senha);
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new { erro = "Credenciais inválidas." });

        return Ok(new { token, tipo = "Bearer" });
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPacienteDto dto)
    {
        var paciente = new Paciente(
            Guid.NewGuid(),
            dto.Nome.Trim(),
            dto.Cpf,
            dto.Email.Trim().ToLowerInvariant(),
            dto.Senha
        );

        var id = await _service.RegistrarPacienteAsync(paciente);
        return CreatedAtAction(nameof(Registrar), new { id }, new { id });
    }
}
