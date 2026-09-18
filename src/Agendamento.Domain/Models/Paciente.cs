namespace Agendamento.Domain.Models;
public class Paciente : Entity
{
    public string Nome { get; set; }
    public DateOnly DataNascimento { get; set; }
    public string CPF { get; set; }
    public string? RG { get; set; }
    public StatusGenero? StatusGenero { get; set; }
    public StatusEstadoCivil? StatusEstadoCivil { get; set; }
    public StatusPaciente StatusPaciente { get; set; }
    public Endereco Endereco { get; set; }
    public Contato Contato { get; set; }

    public Paciente() { }

    public Paciente(string nome, DateOnly dataNascimento, string cpf, string rg, StatusGenero statusGenero, StatusEstadoCivil statusEstadoCivil, StatusPaciente statusPaciente, Endereco endereco, Contato contato)
    {
        Nome = nome;
        DataNascimento = dataNascimento;
        CPF = cpf;
        RG = rg;
        StatusGenero = statusGenero;
        StatusEstadoCivil = statusEstadoCivil;
        StatusPaciente = statusPaciente;
        Endereco = endereco;
        Contato = contato;
    }

}