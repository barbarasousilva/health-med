using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Interfaces;
using HealthMed.Application.Services;

namespace HealthMed.Tests.Unit.Services;

public class MedicoServiceTests
{
    private readonly Mock<IMedicoRepository> _repoMock;
    private readonly MedicoService _service;

    public MedicoServiceTests()
    {
        _repoMock = new Mock<IMedicoRepository>();
        _service = new MedicoService(_repoMock.Object);
    }

    [Fact(DisplayName = "RegistrarMedicoAsync deve adicionar médico se CRM for único")]
    public async Task RegistrarMedico_DeveAdicionarMedico()
    {
        var medico = new Medico(
            Guid.NewGuid(), "Dra. Ana", "CRM123", "Clínica Geral",
            "senhahash", "São Paulo", "SP"
        );

        _repoMock.Setup(r => r.ObterPorCRMAsync("CRM123"))
                 .ReturnsAsync((Medico?)null);

        var id = await _service.RegistrarMedicoAsync(medico);

        _repoMock.Verify(r => r.AdicionarAsync(It.Is<Medico>(m =>
            m.CRM == "CRM123" &&
            m.Nome == "Dra. Ana"
        )), Times.Once);

        Assert.Equal(medico.Id, id);
    }

    [Fact(DisplayName = "RegistrarMedicoAsync deve lançar exceção se CRM já existir")]
    public async Task RegistrarMedico_DeveFalhar_SeCRMExistir()
    {
        var medico = new Medico(Guid.NewGuid(), "Dr. João", "CRM123", "Cardiologia", "hash", "BH", "MG");

        _repoMock.Setup(r => r.ObterPorCRMAsync("CRM123"))
                 .ReturnsAsync(medico);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.RegistrarMedicoAsync(medico));
    }

    [Fact(DisplayName = "BuscarAsync deve retornar lista de médicos")]
    public async Task BuscarAsync_DeveRetornarLista()
    {
        var resultado = new List<Medico>
    {
        new Medico(Guid.NewGuid(), "Dr. A", "CRM001", "Ortopedia", "hash", "SP", "SP"),
        new Medico(Guid.NewGuid(), "Dr. B", "CRM002", "Clínico", "hash", "RJ", "RJ")
    };

        _repoMock.Setup(r => r.BuscarAsync(It.IsAny<FiltroMedico>()))
                 .ReturnsAsync(resultado);

        var retorno = await _service.BuscarAsync(new FiltroMedico());

        Assert.Equal(2, retorno.Count());
    }
}
