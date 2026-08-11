using Agendamento.Domain.Models;

namespace Agendamento.Domain.Interfaces;

public interface IAgendaRepository : IRepository<Agenda>
{
    Task<bool> ExisteConflitoHorario(
        Guid profissionalId,
        DateTime dataInicio,
        DateTime dataFim);

    Task<List<Agenda>> ObterAgendamentosPorProfissional(Guid profissionalId);

    Task<bool> VerificarConflitoProfissional(Guid profissionalId, DateTime inicio, DateTime fim);
    Task Adicionar(Agenda agenda);
}
