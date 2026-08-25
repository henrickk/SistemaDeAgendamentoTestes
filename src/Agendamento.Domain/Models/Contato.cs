namespace Agendamento.Domain.Models;

public class Contato : Entity
{
    public string? Email { get; set; }
    public string NumeroCelular { get; set; }

    public Contato(string email, string numeroCelular)
    {
        Email = email;
        NumeroCelular = numeroCelular;
    }
}