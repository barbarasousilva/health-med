using System.Net.Http.Json;
using Xunit;

namespace HealthMed.Tests.Integration;

public class BuscarMedicosTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BuscarMedicosTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Paciente pode buscar m�dicos pelo nome")]
    public async Task PacienteBuscaMedicos()
    {
        var response = await _client.GetAsync("/api/medicos?nome=M�dico");
        Assert.True(response.IsSuccessStatusCode);
    }
}