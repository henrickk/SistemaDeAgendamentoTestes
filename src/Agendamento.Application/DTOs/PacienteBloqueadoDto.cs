using Agendamento.Domain.Models;

namespace Agendamento.Application.DTOs;
public class PacienteBloqueadoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public StatusPaciente StatusPaciente { get; set; }

}