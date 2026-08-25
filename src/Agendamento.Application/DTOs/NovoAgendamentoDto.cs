namespace Agendamento.Application.DTOs;
public class NovoAgendamentoDto
{
    public Guid PacienteId { get; set; }
    public Guid ProfissionalId { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string? Observacao { get; set; }

}
