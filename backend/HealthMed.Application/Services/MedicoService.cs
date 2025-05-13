using HealthMed.Application.DTOs;
using HealthMed.Domain.Interfaces;

namespace HealthMed.Application.Services;

public class MedicoService : IMedicoService
{
    private readonly IMedicoRepository _medicoRepository;

    public MedicoService(IMedicoRepository medicoRepository)
    {
        _medicoRepository = medicoRepository;
    }

    public async Task<Guid> RegistrarMedicoAsync(Medico medico)
    {
        var existente = await _medicoRepository.ObterPorCRMAsync(medico.CRM);
        if (existente != null)
            throw new InvalidOperationException("Já existe um médico com este CRM.");

        await _medicoRepository.AdicionarAsync(medico);

        return medico.Id;
    }


    public async Task<IEnumerable<Medico>> BuscarAsync(FiltroMedico filtro)
    {
        return await _medicoRepository.BuscarAsync(filtro);
    }

}