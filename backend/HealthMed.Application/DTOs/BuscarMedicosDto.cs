namespace HealthMed.Application.DTOs
{
    public class BuscarMedicosDto
    {
        public Guid Id { get; set; }
        public DateTime DataHora { get; set; }
        public DateTime DataHoraFim { get; set; }
        public string NomeMedico { get; set; } = string.Empty;
        public string Especialidade { get; set; } = string.Empty;
    }
}