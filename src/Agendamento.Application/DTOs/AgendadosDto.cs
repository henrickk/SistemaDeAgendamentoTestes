using Agendamento.Domain.Models;

namespace Agendamento.Application.DTOs;

public class AgendadosDto
{
    public Guid Id { get; set; } 
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public StatusAgendamento StatusAgendamento { get; set; }
    public string? Observacao { get; set; }
    public string PacienteNome { get; set; }
    public ContatoDto PacienteContato { get; set; }
    public string ProfissionalNome { get; set; }
    public string CRO { get; set; }
}
