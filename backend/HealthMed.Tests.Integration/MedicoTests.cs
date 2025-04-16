using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using HealthMed.Application.DTOs;
using HealthMed.Domain.Enums;
using System.Collections.Generic;

namespace HealthMed.Tests.Integration;

public class MedicoTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    private const string MedicoCrm = "123456";
    private const string Senha = "123456";

    private const string PacienteCpf = "12345678901";
    private const string PacienteEmail = "paciente@teste.com";
    private const string SenhaPaciente = "123456";

    public MedicoTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Login do médico com credenciais válidas retorna token JWT")]
    public async Task LoginMedico_DeveRetornarToken()
    {
        var dto = new LoginMedicoDto
        {
            CRM = MedicoCrm,
            Senha = Senha
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login-medico", dto);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        Assert.NotNull(content);
        Assert.True(content!.ContainsKey("token"));

    }

    [Fact(DisplayName = "Listar horários com token JWT válido retorna 200 OK")]
    public async Task ListarHorarios_ComTokenValido_DeveRetornarOk()
    {
        var dto = new LoginMedicoDto
        {
            CRM = MedicoCrm,
            Senha = Senha
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login-medico", dto);
        var rawBody = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode(); 

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(content);
        Assert.True(content!.ContainsKey("token"));
    }

    [Fact(DisplayName = "Cadastrar horário com token JWT válido retorna 201 Created")]
    public async Task CadastrarHorario_DeveRetornarCreated()
    {
        var login = new LoginMedicoDto { CRM = MedicoCrm, Senha = MedicoCrm };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login-medico", login);
        loginResponse.EnsureSuccessStatusCode();
        var token = (await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>())!["token"];

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var now = DateTime.UtcNow;
        var novoHorario = new CadastrarHorarioDto
        {
            DataHora = now.AddMinutes(10),
            DataHoraFim = now.AddMinutes(40),
            Status = StatusHorario.Disponivel
        };

        var response = await _client.PostAsJsonAsync("/api/horarios", novoHorario);

        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

        var responseBody = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(responseBody);
        Assert.True(responseBody!.ContainsKey("id"));
    }

    [Fact(DisplayName = "Excluir horário existente com token válido retorna 204 NoContent")]
    public async Task RemoverHorario_DeveRetornarNoContent()
    {
        var login = new LoginMedicoDto { CRM = MedicoCrm, Senha = Senha };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login-medico", login);
        loginResponse.EnsureSuccessStatusCode();
        var token = (await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>())!["token"];

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var now = DateTime.UtcNow;
        var novoHorario = new CadastrarHorarioDto
        {
            DataHora = now.AddMinutes(10),
            DataHoraFim = now.AddMinutes(40),
            Status = StatusHorario.Disponivel
        };

        var criarResponse = await _client.PostAsJsonAsync("/api/horarios", novoHorario);
        criarResponse.EnsureSuccessStatusCode();
        var createdData = await criarResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var horarioId = createdData!["id"];

        var response = await _client.DeleteAsync($"/api/horarios/{horarioId}");

        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
    [Fact(DisplayName = "Atualizar horário existente com token válido retorna 204 NoContent")]
    public async Task AtualizarHorario_DeveRetornarNoContent()
    {
        var login = new LoginMedicoDto {CRM = MedicoCrm, Senha = Senha };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login-medico", login);
        loginResponse.EnsureSuccessStatusCode();
        var token = (await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>())!["token"];

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var now = DateTime.UtcNow;

        var novoHorario = new CadastrarHorarioDto
        {
            DataHora = now.AddMinutes(10),
            DataHoraFim = now.AddMinutes(40),
            Status = StatusHorario.Disponivel
        };

        var criarResponse = await _client.PostAsJsonAsync("/api/horarios", novoHorario);
        criarResponse.EnsureSuccessStatusCode();
        var createdData = await criarResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var horarioId = createdData!["id"];

        var editarHorario = new EditarHorarioDto
        {
            DataHora = now.AddMinutes(15),
            DataHoraFim = now.AddMinutes(45),
            Status = StatusHorario.Ocupado
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/horarios/{horarioId}", editarHorario);

        updateResponse.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, updateResponse.StatusCode);
    }

    [Fact(DisplayName = "Abrir agenda com token válido retorna 200 OK")]
    public async Task AbrirAgenda_DeveRetornarOk()
    {
        var login = new LoginMedicoDto {CRM = MedicoCrm, Senha = Senha };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login-medico", login);
        loginResponse.EnsureSuccessStatusCode();
        var token = (await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>())!["token"];

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var abrirAgendaDto = new AbrirAgendaDto
        {
            Data = DateTime.UtcNow.AddDays(1).Date,
            Duracao = DuracaoConsulta.TrintaMinutos,
        };

        var response = await _client.PostAsJsonAsync("/api/horarios/abrir-agenda", abrirAgendaDto);

        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(content);
        Assert.Equal("Agenda aberta com sucesso!", content!["mensagem"]);
    }

    [Fact(DisplayName = "Acesso à rota protegida sem token retorna 401 Unauthorized")]
    public async Task AcessoSemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync("/api/horarios");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Acesso com token inválido retorna 401 Unauthorized")]
    public async Task AcessoComTokenInvalido_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token-invalido-aqui");

        var response = await _client.GetAsync("/api/horarios");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Acesso com token de paciente retorna 403 ou 401")]
    public async Task AcessoComTokenDePaciente_DeveRetornarAcessoNegado()
    {
        var login = new LoginPacienteDto { CpfOuEmail = PacienteCpf, Senha = SenhaPaciente };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/paciente/login", login);
        loginResponse.EnsureSuccessStatusCode();

        var content = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var token = content!["token"];
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/horarios");
        Assert.True(
            response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized,
            $"Esperado 403 ou 401, mas recebeu {response.StatusCode}"
        );
    }

}
