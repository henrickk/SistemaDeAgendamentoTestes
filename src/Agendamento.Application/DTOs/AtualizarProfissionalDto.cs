using Agendamento.Domain.Models;

namespace Agendamento.Application.DTOs;
public class AtualizarProfissionalDto
{
    public string Nome { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public ContatoDto Contato { get; set; }
    public EnderecoDto Endereco { get; set; }
}
