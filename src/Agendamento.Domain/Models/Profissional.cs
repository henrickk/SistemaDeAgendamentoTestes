namespace Agendamento.Domain.Models;

public class Profissional : Entity
{
    public string Nome { get; set; }
    public string CRO { get; set; }
    public string CPF { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public Contato Contato { get; set; }
    public Endereco Endereco { get; set; }

    public Profissional() { }

    public Profissional(string nome, string cro, string cpf, TimeOnly horaInicio, TimeOnly horaFim, Contato contato, Endereco endereco)
    {
        Nome = nome;
        CRO = cro;
        CPF = cpf;
        HoraInicio = horaInicio;
        HoraFim = horaFim;
        Contato = contato;
        Endereco = endereco;
    }
}
