using HealthMed.Application.DTOs;
using HealthMed.Application.Services;
using HealthMed.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthMed.Api.Controllers;

[Authorize(Roles = "medico")]
[ApiController]
[Route("api/horarios")]
public class HorariosDisponiveisController : ControllerBase
{
    private readonly IHorarioDisponivelService _service;

    public HorariosDisponiveisController(IHorarioDisponivelService service)
    {
        _service = service;
    }

    [HttpGet("up")]
    public IActionResult Up()
    {
        var medicoId = User.GetMedicoId();
        var crm = User.GetCRM();
        var nome = User.GetNome();

        return Ok(new
        {
            status = "ok",
            rota = "horarios",
            medicoId,
            crm,
            nome,
        });
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var medicoId = User.GetMedicoId(); // Log para verificar se o id está sendo extraído corretamente
        var role = User.FindFirstValue(ClaimTypes.Role); // Verifica o role

        var horarios = await _service.ListarPorMedicoAsync(medicoId);
        return Ok(horarios);
    }


    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarHorarioDto dto)
    {
        var medicoId = User.GetMedicoId();
        var id = await _service.AdicionarAsync(medicoId, dto.DataHora, dto.DataHoraFim, dto.Status);
        return CreatedAtAction(nameof(Listar), new { id }, new { id });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] EditarHorarioDto dto)
    {
        var medicoId = User.GetMedicoId();
        await _service.AtualizarAsync(id, medicoId, dto.DataHora, dto.DataHoraFim, dto.Status);
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        var medicoId = User.GetMedicoId();
        await _service.RemoverAsync(id, medicoId);
        return NoContent();
    }

    [HttpPost("abrir-agenda")]
    public async Task<IActionResult> AbrirAgenda([FromBody] AbrirAgendaDto dto)
    {
        var medicoId = User.GetMedicoId();
        await _service.AbrirAgendaAsync(medicoId, dto.Data, dto.Duracao.ToTimeSpan());
        return Ok(new { mensagem = "Agenda aberta com sucesso!" });
    }

}
