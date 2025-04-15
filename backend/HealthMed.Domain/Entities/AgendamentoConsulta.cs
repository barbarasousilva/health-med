namespace HealthMed.Domain.Entities
{
    public class AgendamentoConsulta
    {
        public Guid Id { get; private set; }
        public Guid PacienteId { get; private set; }
        public Guid MedicoId { get; private set; }
        public Guid HorarioDisponivelId { get; private set; }

        public AgendamentoConsulta(Guid id, Guid pacienteId, Guid medicoId, Guid horarioDisponivelId)
        {
            Id = id;
            PacienteId = pacienteId;
            MedicoId = medicoId;
            HorarioDisponivelId = horarioDisponivelId;
        }
    }
}
