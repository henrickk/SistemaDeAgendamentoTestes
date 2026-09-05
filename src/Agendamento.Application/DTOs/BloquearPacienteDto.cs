using Agendamento.Domain.Models;

namespace Agendamento.Application.DTOs;
public class BloquearPacienteDto
{
    public Guid PacienteId { get; set; }
    public string Nome { get; set; }
    public StatusPaciente StatusPaciente { get; set; }
}
