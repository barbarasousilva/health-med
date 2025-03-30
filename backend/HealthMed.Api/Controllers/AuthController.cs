using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("up")]
    public IActionResult Up()
    {
        return Ok(new { status = "ok", rota = "login-medico" });
    }

    [HttpPost("login-medico")]
    public async Task<IActionResult> LoginMedico([FromBody] LoginMedicoDto loginDto)
    {
        var token = await _authService.AutenticarMedicoAsync(loginDto);

        if (token == null)
            return Unauthorized(new { mensagem = "CRM ou senha inválidos." });

        return Ok(new
        {
            token,
            tipo = "Bearer"
        });
    }
}
