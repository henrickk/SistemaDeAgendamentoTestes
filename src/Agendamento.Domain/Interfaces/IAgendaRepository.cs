using Agendamento.Domain.Models;

namespace Agendamento.Domain.Interfaces;

public interface IAgendaRepository : IRepository<Agenda>
{
    Task<Agenda> Adicionar(Agenda agenda);

    Task<bool> ExisteConflitoHorario(
        Guid profissionalId,
        DateTime dataInicio,
        DateTime dataFim);

    Task<List<Agenda>> ObterAgendamentosPorProfissional(Guid profissionalId);

}
