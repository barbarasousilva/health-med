public class Medico
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string CRM { get; private set; }
    public string Especialidade { get; private set; }
    public string SenhaHash { get; private set; }
    public string Cidade { get; private set; }
    public string UF { get; private set; }

    public Medico(Guid id, string nome, string crm, string especialidade, string senhaHash, string cidade, string uf)
    {
        Id = id;
        Nome = nome;
        CRM = crm;
        Especialidade = especialidade;
        SenhaHash = senhaHash;
        Cidade = cidade;
        UF = uf;
    }
}
