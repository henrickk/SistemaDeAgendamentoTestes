namespace Agendamento.Domain.Models;

public class Profissional : Entity
{
    public Profissional() { }

    public string Nome { get; set; }
    public string CRO { get; set; }
    public string CPF { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public Contato Contato { get; set; }
    public Endereco Endereco { get; set; }
}

