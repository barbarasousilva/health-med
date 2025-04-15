namespace HealthMed.Domain.Entities;

public class Paciente
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Cpf { get; private set; }
    public string SenhaHash { get; private set; }

    public Paciente() { }

    public Paciente(Guid id, string nome, string email, string cpf, string senhaHash)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Cpf = cpf;
        SenhaHash = senhaHash;
    }
    public Paciente(Guid id, string nome, string cpf, string email)
    {
        Id = id;
        Nome = nome;
        Cpf = cpf;
        Email = email;
        SenhaHash = ""; // será preenchido depois
    }

    public void AtualizarSenha(string novaSenhaHash)
    {
        SenhaHash = novaSenhaHash;
    }

    public void AtualizarDados(string nome, string email)
    {
        Nome = nome;
        Email = email;
    }
}
