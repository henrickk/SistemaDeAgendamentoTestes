using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;

namespace Agendamento.Infrastructure.Repository;
public class AgendaRepositor : Repository<Agenda>, IAgendaRepository
{
    public AgendaRepositor()
    {
    }

    public Task<bool> ExisteConflitoHorario(Guid profissionalId, DateTime dataInicio, DateTime dataFim)
    {
        throw new NotImplementedException();
    }

    public Task<List<Agenda>> ObterAgendamentosPorProfissional(Guid profissionalId)
    {
        throw new NotImplementedException();
    }

    Task<Agenda> IAgendaRepository.Adicionar(Agenda agenda)
    {
        throw new NotImplementedException();
    }
}
