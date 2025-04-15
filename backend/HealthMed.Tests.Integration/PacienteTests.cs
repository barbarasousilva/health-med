using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using HealthMed.Application.DTOs;

namespace HealthMed.Tests.Integration;

public class PacienteTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PacienteTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Registrar paciente com dados válidos retorna Created")]
    public async Task Registrar_DeveRetornarCreated()
    {
        var identificador = Guid.NewGuid().ToString("N")[..6];
        var email = $"paciente_{identificador}@teste.com";
        var cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => new Random().Next(0, 10).ToString()));
        Console.WriteLine($"CPF Registrar Paciente: {cpf}");

        var registro = new RegistrarPacienteDto
        {
            Nome = "Paciente Teste",
            Email = email,
            Cpf = cpf,
            Senha = "123456"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/paciente/registrar", registro);
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.Conflict);
    }

    [Fact(DisplayName = "Login deve retornar token JWT se paciente existir com senha correta")]
    public async Task Login_DeveRetornarToken_SeCredenciaisForemValidas()
    {
        var identificador = Guid.NewGuid().ToString("N")[..6];
        var email = $"paciente_{identificador}@teste.com";
        var cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => new Random().Next(0, 10).ToString()));
        const string senha = "123456";
        Console.WriteLine($"CPF Login deve Retornar token: {cpf}");

        // Garante que o paciente existe
        await _client.PostAsJsonAsync("/api/auth/paciente/registrar", new RegistrarPacienteDto
        {
            Nome = "Paciente Teste",
            Email = email,
            Cpf = cpf,
            Senha = senha
        });

        var login = new LoginPacienteDto { CpfOuEmail = email, Senha = senha };
        var response = await _client.PostAsJsonAsync("/api/auth/paciente/login", login);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(content);
        Assert.True(content.ContainsKey("token"));
        Assert.Contains(".", content["token"]);
    }

}
