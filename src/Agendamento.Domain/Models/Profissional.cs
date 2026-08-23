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

    protected Profissional() { }

    public Profissional(string nome, DateOnly dateOnly, string cro, string v, string v1, Contato contato, TimeOnly horaInicio, TimeOnly horaFim, string cpf, Endereco endereco)
    {
        Nome = nome;
        CRO = cro;
        Contato = contato;
        HoraInicio = horaInicio;
        HoraFim = horaFim;
        CPF = cpf;
        Endereco = endereco;
    }
}

