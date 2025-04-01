using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
using HealthMed.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpGet("up")]
    public IActionResult Up()
    {
        return Ok(new { status = "ok", rota = "login-medico" });
    }

    [HttpPost("login-medico")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginMedico([FromBody] LoginMedicoDto dto)
    {
        var token = await _auth.AutenticarMedicoAsync(dto.CRM, dto.Senha);

        if (token == null)
            return Unauthorized(new { mensagem = "Credenciais inválidas." });

        return Ok(new { token, tipo = "Bearer" });
    }
}
