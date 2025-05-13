using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HealthMed.Application.DTOs;
using HealthMed.Domain.Enums;

namespace HealthMed.Tests.Integration;

public class TestUtils
{
    private readonly HttpClient _client;

    public TestUtils(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> AutenticarPacienteAsync()
    {
        var dto = new LoginPacienteDto
        {
            CpfOuEmail = "paciente@teste.com",
            Senha = "123456"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/paciente/login", dto);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return json!["token"];
    }

    public async Task<string> AutenticarMedicoAsync()
    {
        var dto = new LoginMedicoDto
        {
            CRM = "123456",
            Senha = "123456"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login-medico", dto);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return json!["token"];
    }

}
