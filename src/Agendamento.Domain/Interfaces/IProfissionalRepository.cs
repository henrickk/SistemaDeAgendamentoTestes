using Agendamento.Domain.Models;

namespace Agendamento.Domain.Interfaces;

public interface IProfissionalRepository : IRepository<Profissional>
{
    Task<Profissional> ObterPorCRO(string cro);
    Task<List<Profissional>> ObterPorNome(string nome);
}
