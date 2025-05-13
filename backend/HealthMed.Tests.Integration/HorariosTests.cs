using System.Net.Http.Headers;
using System.Net.Http.Json;
using HealthMed.Application.DTOs;
using HealthMed.Domain.Enums;
using Xunit;

namespace HealthMed.Tests.Integration;

public class HorariosTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestUtils _utils;

    public HorariosTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _utils = new TestUtils(_client);
    }

    [Fact(DisplayName = "Médico pode cadastrar horário disponível")]
    public async Task MedicoPodeCadastrarHorario()
    {
        var token = await _utils.AutenticarMedicoAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CadastrarHorarioDto
        {
            DataHora = DateTime.UtcNow.AddHours(1),
            DataHoraFim = DateTime.UtcNow.AddHours(2),
            Status = StatusHorario.Disponivel
        };

        var response = await _client.PostAsJsonAsync("/api/horarios", dto);

        Assert.True(response.IsSuccessStatusCode, $"Erro ao cadastrar horário: {await response.Content.ReadAsStringAsync()}");
    }

}