using System.Net;
using System.Net.Http.Json;
using HealthMed.Application.DTOs;
using Xunit;

namespace HealthMed.Tests.Integration;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Login do médico com credenciais válidas retorna JWT")]
    public async Task LoginMedico_DeveRetornarToken()
    {
        var dto = new LoginMedicoDto { CRM = "123456", Senha = "123456" };
        var response = await _client.PostAsJsonAsync("/api/auth/login-medico", dto);
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(content);
        Assert.True(content!.ContainsKey("token"));
    }

    [Fact(DisplayName = "Login do paciente com e-mail retorna JWT")]
    public async Task LoginPaciente_Email_DeveRetornarToken()
    {
        var dto = new LoginPacienteDto { CpfOuEmail = "paciente@teste.com", Senha = "123456" };
        var response = await _client.PostAsJsonAsync("/api/auth/paciente/login", dto);
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(content);
        Assert.True(content!.ContainsKey("token"));
    }
}
