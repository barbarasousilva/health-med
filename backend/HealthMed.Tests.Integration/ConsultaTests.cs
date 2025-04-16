using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;
using HealthMed.Application.DTOs;
using HealthMed.Domain.Enums;
using System.Text.Json;

namespace HealthMed.Tests.Integration;

public class ConsultaTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _clientMedico;
    private readonly HttpClient _clientPaciente;

    public ConsultaTests(CustomWebApplicationFactory factory)
    {
        _clientMedico = factory.CreateClient();
        _clientPaciente = factory.CreateClient();
    }

    private async Task<string> AutenticarMedicoAsync()
    {
        var login = new LoginMedicoDto { CRM = "123456", Senha = "123456" };
        var response = await _clientMedico.PostAsJsonAsync("/api/auth/login-medico", login);
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return content!["token"];
    }

    private async Task<string> AutenticarPacienteAsync()
    {
        var identificador = Guid.NewGuid().ToString("N")[..6];
        var email = $"paciente_{identificador}@teste.com";
        var cpf = $"000000{identificador}".Substring(0, 11);
        const string senha = "123456";

        await _clientPaciente.PostAsJsonAsync("/api/auth/paciente/registrar", new RegistrarPacienteDto
        {
            Nome = "Paciente Teste",
            Email = email,
            Cpf = cpf,
            Senha = senha
        });

        var login = new LoginPacienteDto { CpfOuEmail = email, Senha = senha };
        var response = await _clientPaciente.PostAsJsonAsync("/api/auth/paciente/login", login);
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return content!["token"];
    }

    private async Task<Guid> CriarHorarioAsync(string tokenMedico)
    {
        _clientMedico.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenMedico);

        var dto = new CadastrarHorarioDto
        {
            DataHora = DateTime.UtcNow.AddHours(1),
            DataHoraFim = DateTime.UtcNow.AddHours(2),
            Status = StatusHorario.Disponivel
        };

        var response = await _clientMedico.PostAsJsonAsync("/api/horarios", dto);
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return Guid.Parse(content!["id"]);
    }

    private async Task<Guid> AgendarConsultaAsync(string tokenPaciente, Guid idMedico, Guid idHorario)
    {
        _clientPaciente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenPaciente);

        var dto = new AgendarConsultaDto
        {
            IdMedico = idMedico,
            IdHorarioDisponivel = idHorario
        };

        var response = await _clientPaciente.PostAsJsonAsync("/consultas", dto);
        var body = await response.Content.ReadAsStringAsync();

        var consultaId = JsonDocument.Parse(body).RootElement
                          .GetProperty("mensagem").GetString();

        return idHorario;
    }

    [Fact(DisplayName = "Paciente pode cancelar uma consulta com justificativa")]
    public async Task Paciente_PodeCancelarConsulta()
    {
        var tokenMedico = await AutenticarMedicoAsync();
        var tokenPaciente = await AutenticarPacienteAsync();

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenMedico);
        var medicoId = jwt.Claims.First(c => c.Type == "id").Value;

        var idHorario = await CriarHorarioAsync(tokenMedico);
        var idConsulta = await AgendarConsultaAsync(tokenPaciente, Guid.Parse(medicoId), idHorario);

        _clientPaciente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenPaciente);

        var dto = new CancelarConsultaDto { Justificativa = "Paciente teve imprevisto" };
        var response = await _clientPaciente.PutAsJsonAsync($"/consultas/{idConsulta}/cancelar", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
