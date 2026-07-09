using Agendamento.Domain.Models;

namespace Agendamento.Domain.Interfaces;

public interface IAgendaRepository : IRepository<Agenda>
{
    Task AdicionarAsync(Agenda agenda);

    Task<bool> ExisteConflitoHorarioAsync(
        Guid profissionalId,
        DateTime dataInicio,
        DateTime dataFim);

    Task<List<Agenda>> ObterAgendamentosPorProfissionalAsync(Guid profissionalId);

}
