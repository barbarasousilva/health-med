namespace HealthMed.Domain.Interfaces
{
    public interface IMedicoRepository
    {
        Task<Medico?> ObterPorCRMAsync(string crm);
        Task AdicionarAsync(Medico medico);
        Task<IEnumerable<Medico>> BuscarAsync(FiltroMedico filtros);
    }
}