using HealthMed.Domain.Enums;
using HealthMed.Domain.Interfaces;

namespace HealthMed.Application.Services
{
    public class AgendamentoConsultaService : IAgendamentoConsultaService
    {
        private readonly IHorarioDisponivelRepository _repository;

        public AgendamentoConsultaService(IHorarioDisponivelRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<object>> BuscarHorariosAsync(
            DateOnly? data,
            StatusHorario? horario,
            string? especialidade,
            Guid? medicoId)
        {
            return await _repository.BuscarHorariosAsync(data, horario, especialidade, medicoId);
        }
    }
}