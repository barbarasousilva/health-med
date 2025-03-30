using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
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
        var token = await _service.AutenticarPacienteAsync(dto);
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new { erro = "Credenciais inválidas." });

        return Ok(new { token, tipo = "Bearer" });
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPacienteDto dto)
    {
        var id = await _service.RegistrarPacienteAsync(dto);
        return CreatedAtAction(nameof(Registrar), new { id }, new { id });
    }
}
