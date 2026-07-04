namespace Agendamento.Domain.Models;

public class Profissional : Entity
{
    public string Nome { get; set; }
    public string CRO { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public Contato Contato { get; set; }

    public Profissional(string nome, string cro, Contato contato, TimeOnly horaInicio, TimeOnly horaFim)
    {
        Nome = nome;
        CRO = cro;
        Contato = contato;
        HoraInicio = horaInicio;
        HoraFim = horaFim;
    }
}
