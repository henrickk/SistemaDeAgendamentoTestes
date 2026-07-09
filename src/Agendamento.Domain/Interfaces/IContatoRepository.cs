using Agendamento.Domain.Models;

namespace Agendamento.Domain.Interfaces;

public interface IContatoRepository : IRepository<Contato>
{
    Task<Contato> ObterPorNumeroContato(Contato contato);
    Task<Contato> ObterPorEmailContato(Contato contato);
}
