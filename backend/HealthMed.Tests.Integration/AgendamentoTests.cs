using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HealthMed.Application.DTOs;
using HealthMed.Domain.Enums;
using Xunit;

namespace HealthMed.Tests.Integration;

public class AgendamentoTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AgendamentoTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Paciente pode agendar consulta")]
    public async Task PacienteAgendaConsulta()
    {
        var pacienteLogin = new { CpfOuEmail = "paciente@teste.com", Senha = "123456" };
        var pacienteResponse = await _client.PostAsJsonAsync("/api/auth/paciente/login", pacienteLogin);
        pacienteResponse.EnsureSuccessStatusCode();
        var pacienteData = await pacienteResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var tokenPaciente = pacienteData!["token"];

        var medicoLogin = new { Crm = "123456", Senha = "123456" };
        var medicoResponse = await _client.PostAsJsonAsync("/api/auth/login-medico", medicoLogin);
        medicoResponse.EnsureSuccessStatusCode();
        var medicoData = await medicoResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var tokenMedico = medicoData!["token"];

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenMedico);

        var inicio = DateTime.UtcNow.AddMinutes(5);
        var fim = inicio.AddMinutes(30);
        var horarioDto = new CadastrarHorarioDto
        {
            DataHora = inicio,
            DataHoraFim = fim,
            Status = StatusHorario.Disponivel
        };

        var horarioResponse = await _client.PostAsJsonAsync("/api/horarios", horarioDto);
        horarioResponse.EnsureSuccessStatusCode();

        var horarioData = await horarioResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var horarioId = Guid.Parse(horarioData!["id"]!.ToString()!);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenPaciente);
        var medicosResponse = await _client.GetAsync("/api/medicos");
        medicosResponse.EnsureSuccessStatusCode();

        var medicos = await medicosResponse.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
        var medico = medicos!.First();
        var medicoId = Guid.Parse(medico["id"]!.ToString()!);

        var agendamentoDto = new AgendarConsultaDto
        {
            IdMedico = medicoId,
            IdHorarioDisponivel = horarioId
        };

        var agendamentoResponse = await _client.PostAsJsonAsync("/api/consultas", agendamentoDto);
        agendamentoResponse.EnsureSuccessStatusCode();

        var resultado = await agendamentoResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var rawBody = await agendamentoResponse.Content.ReadAsStringAsync();

        Assert.NotNull(resultado);
        Assert.True(Guid.TryParse(resultado["id"].ToString(), out var consultaId));
        Assert.NotEqual(Guid.Empty, consultaId);
    }

}
