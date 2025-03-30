namespace HealthMed.Domain.Entities;

public class HorarioDisponivel
{
    public Guid Id { get; private set; }
    public Guid MedicoId { get; private set; }
    public DateTime DataHora { get; private set; }
    public DateTime DataHoraFim { get; private set; }
    public string Status { get; private set; }

    public HorarioDisponivel() { }

    public HorarioDisponivel(Guid id, Guid medicoId, DateTime dataHora, DateTime dataHoraFim, string status)
    {
        if (dataHora >= dataHoraFim)
            throw new ArgumentException("A data/hora de in�cio deve ser anterior � data/hora de fim.");

        if (dataHora <= DateTime.UtcNow)
            throw new ArgumentException("N�o � permitido cadastrar hor�rios no passado.");

        Id = id;
        MedicoId = medicoId;
        DataHora = dataHora;
        DataHoraFim = dataHoraFim;
        Status = status;
    }

    public void Atualizar(DateTime novaDataHoraInicio, DateTime novaDataHoraFim, string novoStatus)
    {
        if (novaDataHoraInicio >= novaDataHoraFim)
            throw new ArgumentException("A data/hora de in�cio deve ser anterior � data/hora de fim.");

        if (novaDataHoraInicio <= DateTime.UtcNow)
            throw new ArgumentException("N�o � permitido atualizar para hor�rios no passado.");

        DataHora = novaDataHoraInicio;
        DataHoraFim = novaDataHoraFim;
        Status = novoStatus;
    }
}
