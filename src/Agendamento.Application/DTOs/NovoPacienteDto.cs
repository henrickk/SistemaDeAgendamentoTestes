using Agendamento.Domain.Models;

namespace Agendamento.Application.DTOs;
public class NovoPacienteDto
{
    public string Nome { get; set; }
    public DateOnly DataNascimento { get; set; }
    public string CPF { get; set; }
    public string RG { get; set; }
    public StatusGenero? StatusGenero { get; set; }
    public StatusEstadoCivil? StatusEstadoCivil { get; set; }
    public StatusPaciente StatusPaciente { get; set; }
    public ContatoDto Contato { get; set; }
    public EnderecoDto Endereco { get; set; }
}
