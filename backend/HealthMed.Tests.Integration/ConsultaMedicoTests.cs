using System.Net.Http.Headers;
using System.Net.Http.Json;
using HealthMed.Domain.Enums;
using HealthMed.Application.DTOs;
using Xunit;

namespace HealthMed.Tests.Integration;

public class ConsultaMedicoTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public ConsultaMedicoTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Médico pode aceitar consulta")]
    public async Task MedicoPodeAceitarConsulta()
    {
        var consultaId = await CriarConsultaPendenteAsync();
        var login = new { Crm = "123456", Senha = "123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login-medico", login);
        loginResponse.EnsureSuccessStatusCode();
        var token = (await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>())!["token"];
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PutAsync($"/api/consultas/{consultaId}/aceitar", null);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.Equal("Consulta aceita.", body?["mensagem"]);
    }

    [Fact(DisplayName = "Médico pode recusar consulta")]
    public async Task MedicoPodeRecusarConsulta()
    {
        var consultaId = await CriarConsultaPendenteAsync();
        var login = new { Crm = "123456", Senha = "123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login-medico", login);
        loginResponse.EnsureSuccessStatusCode();
        var token = (await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>())!["token"];
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PutAsync($"/api/consultas/{consultaId}/recusar", null);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.Equal("Consulta recusada.", body?["mensagem"]);
    }

    private async Task<Guid> CriarConsultaPendenteAsync()
    {
        var pacienteLogin = new { CpfOuEmail = "paciente@teste.com", Senha = "123456" };
        var pacienteResponse = await _client.PostAsJsonAsync("/api/auth/paciente/login", pacienteLogin);
        var pacienteData = await pacienteResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var tokenPaciente = pacienteData!["token"];

        var medicoLogin = new { Crm = "123456", Senha = "123456" };
        var medicoResponse = await _client.PostAsJsonAsync("/api/auth/login-medico", medicoLogin);
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
        var horarioData = await horarioResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var horarioId = Guid.Parse(horarioData!["id"]!.ToString()!);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenPaciente);
        var medicos = await _client.GetFromJsonAsync<List<Dictionary<string, object>>>("/api/medicos");
        var medicoId = Guid.Parse(medicos![0]["id"]!.ToString()!);

        var agendamentoDto = new AgendarConsultaDto { IdMedico = medicoId, IdHorarioDisponivel = horarioId };
        var agendamentoResponse = await _client.PostAsJsonAsync("/api/consultas", agendamentoDto);
        var resultado = await agendamentoResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        return Guid.Parse(resultado!["id"]!.ToString()!);
    }
}

