using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers
{
    [Authorize(Roles = "paciente")]
    [ApiController]
    [Route("api/agendamentos")]
    public class AgendamentosController : ControllerBase
    {
        private readonly IAgendamentoConsultaService _service;

        public AgendamentosController(IAgendamentoConsultaService service)
        {
            _service = service;
        }

        [HttpGet("horarios")]
        public async Task<IActionResult> BuscarHorariosDisponiveis(
            [FromQuery] DateOnly? data,
            [FromQuery] StatusHorario? horario,
            [FromQuery] string? especialidade,
            [FromQuery] Guid? medicoId)
        {
            var resultado = await _service.BuscarHorariosAsync(data, horario, especialidade, medicoId);
            return Ok(resultado);
        }
    }
}