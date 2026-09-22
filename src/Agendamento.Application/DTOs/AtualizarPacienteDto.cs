using Agendamento.Domain.Models;

namespace Agendamento.Application.DTOs;
public class AtualizarPacienteDto
{
    public string Nome { get; set; }
    public DateOnly DataDeNascimento { get; set; }
    public StatusGenero StatusGenero { get; set; }
    public StatusEstadoCivil StatusEstadoCivil { get; set; }
    public StatusPaciente StatusPaciente { get; set; }
    public ContatoDto Contato { get; set; }
    public EnderecoDto Endereco { get; set; }
}
