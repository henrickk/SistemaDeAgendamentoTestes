using Agendamento.Domain.Models;

namespace Agendamento.Application.DTOs;
public class AgendadosDto
{
    public string NomePaciente { get; set; }
    public Contato ContatoPaciente { get; set; }
    public string NomeProfissional { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string StatusAgendamento { get; set; }
    public string? Observacao { get; set; }

}
