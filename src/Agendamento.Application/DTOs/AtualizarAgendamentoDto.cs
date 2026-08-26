using Agendamento.Domain.Models;

namespace Agendamento.Application.DTOs;
public class AtualizarAgendamentoDto
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public StatusAgendamento StatusAgendamento { get; set; }
    public Guid ProfissionalId { get; set; }
    public string? Observacao { get; set; }
}
