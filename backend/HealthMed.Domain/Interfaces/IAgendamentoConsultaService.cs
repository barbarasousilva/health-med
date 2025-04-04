using HealthMed.Domain.Enums;

namespace HealthMed.Domain.Interfaces
{
    public interface IAgendamentoConsultaService
    {
        Task<IEnumerable<object>> BuscarHorariosAsync(
            DateOnly? data,
            StatusHorario? horario,
            string? especialidade,
            Guid? medicoId);
    }
}