using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using HealthMed.Application.DTOs;
using System.Collections.Generic;

namespace HealthMed.Tests.Integration;

public class PacienteTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    private const string PacienteCpf = "12345678901";
    private const string PacienteEmail = "paciente@teste.com";
    private const string Senha = "123456";

    public PacienteTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Login com e-mail retorna token JWT se credenciais forem válidas")]
    public async Task Login_ComEmail_DeveRetornarToken()
    {
        var login = new LoginPacienteDto
        {
            CpfOuEmail = PacienteEmail,
            Senha = Senha
        };

        var response = await _client.PostAsJsonAsync("/api/auth/paciente/login", login);
        var body = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, $"Resposta: {response.StatusCode}, Conteúdo: {body}");

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        Assert.NotNull(content);
        Assert.True(content!.ContainsKey("token"));
    }

    [Fact(DisplayName = "Login com CPF retorna token JWT se credenciais forem válidas")]
    public async Task Login_ComCpf_DeveRetornarToken()
    {
        var login = new LoginPacienteDto
        {
            CpfOuEmail = PacienteCpf,
            Senha = Senha
        };

        var response = await _client.PostAsJsonAsync("/api/auth/paciente/login", login);
        var body = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, $"Resposta: {response.StatusCode}, Conteúdo: {body}");

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        Assert.NotNull(content);
        Assert.True(content!.ContainsKey("token"));
    }
}
