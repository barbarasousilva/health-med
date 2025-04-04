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
        var dto = new RegistrarPacienteDto
        {
            Nome = "Paciente Teste",
            Email = $"paciente{Guid.NewGuid():N}@teste.com",
            Cpf = "12345678900",
            Senha = "senha123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/paciente/registrar", dto);

        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact(DisplayName = "Login deve retornar token JWT se paciente existir com senha correta")]
    public async Task Login_DeveRetornarToken_SeCredenciaisForemValidas()
    {
        var email = $"paciente{Guid.NewGuid():N}@teste.com";
        var senha = "senha123";
        var cpf = "12345678900";

        var dtoRegistro = new RegistrarPacienteDto
        {
            Nome = "Paciente Teste",
            Email = email,
            Cpf = cpf,
            Senha = senha
        };

        var responseRegistro = await _client.PostAsJsonAsync("/api/auth/paciente/registrar", dtoRegistro);
        responseRegistro.EnsureSuccessStatusCode();

        var dtoLogin = new LoginPacienteDto
        {
            CpfOuEmail = cpf,
            Senha = senha
        };

        var responseLogin = await _client.PostAsJsonAsync("/api/auth/paciente/login", dtoLogin);
        responseLogin.EnsureSuccessStatusCode();

        var resposta = await responseLogin.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        Assert.NotNull(resposta);
        Assert.True(resposta!.ContainsKey("token"));

    }
}
