namespace HealthMed.Domain.Entities;

public class Medico
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string CRM { get; private set; }
    public string Especialidade { get; private set; }
    public string SenhaHash { get; private set; }

    public Medico() { }

    public Medico(string nome, string crm, string especialidade, string senhaHash)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        CRM = crm;
        Especialidade = especialidade;
        SenhaHash = senhaHash;
    }

    public void AtualizarSenha(string novoHash)
    {
        SenhaHash = novoHash;
    }
}
