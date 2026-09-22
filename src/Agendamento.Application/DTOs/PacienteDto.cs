using Agendamento.Domain.Models;

namespace Agendamento.Application.DTOs;
public class PacienteDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public DateOnly DataNascimento { get; set; }
    public StatusPaciente StatusPaciente { get; set; }
    public ContatoDto Contato { get; set; }
}
