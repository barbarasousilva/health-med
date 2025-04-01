using HealthMed.Domain.Enums;

namespace HealthMed.Domain.Entities;

public class HorarioDisponivel
{
    public Guid Id { get; private set; }
    public Guid MedicoId { get; private set; }
    public DateTime DataHora { get; private set; }
    public DateTime DataHoraFim { get; private set; }
    public StatusHorario Status { get; private set; }

    public HorarioDisponivel() { }

    public HorarioDisponivel(Guid id, Guid medicoId, DateTime dataHora, DateTime dataHoraFim, StatusHorario status)
    {
        if (dataHora >= dataHoraFim)
            throw new ArgumentException("A data/hora de início deve ser anterior à data/hora de fim.");

        if (dataHora <= DateTime.UtcNow)
            throw new ArgumentException("Não é permitido cadastrar horários no passado.");

        Id = id;
        MedicoId = medicoId;
        DataHora = dataHora;
        DataHoraFim = dataHoraFim;
        Status = status;
    }

    public void Atualizar(DateTime novaDataHoraInicio, DateTime novaDataHoraFim, StatusHorario novoStatus)
    {
        if (novaDataHoraInicio >= novaDataHoraFim)
            throw new ArgumentException("A data/hora de início deve ser anterior à data/hora de fim.");

        if (novaDataHoraInicio <= DateTime.UtcNow)
            throw new ArgumentException("Não é permitido atualizar para horários no passado.");

        DataHora = novaDataHoraInicio;
        DataHoraFim = novaDataHoraFim;
        Status = novoStatus;
    }
}
