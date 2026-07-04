namespace Agendamento.Domain.Models;

public abstract class Entity
{
    public Guid Id { get; private set; }
    public DateTime DataCadastro { get; private set; } = DateTime.Now;
    protected Entity()
    {
        Id = Guid.NewGuid();
        DataCadastro = DateTime.Now;
    }
}

