using Agendamento.Domain.Models;

namespace Agendamento.Domain.Interfaces;

public interface IProfissionalRepository : IRepository<Profissional>
{
    Task<Profissional> ObterPorId(int id);
    Task<List<Profissional>> ObterTodos();
    Task<Profissional> ObterPorCRO(string cro);
    Task<Profissional> ObterPorNome(string nome);
}
