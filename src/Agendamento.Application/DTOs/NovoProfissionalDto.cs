using Agendamento.Domain.Models;

namespace Agendamento.Application.DTOs;
public class NovoProfissionalDto
{
    public string Nome { get; set; }
    public string CRO { get; set; }
    public string CPF { get; set; }
    public Contato Contato { get; set; }
    public Endereco Endereco { get; set; }
}
