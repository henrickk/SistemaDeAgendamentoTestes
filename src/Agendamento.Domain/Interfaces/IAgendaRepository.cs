using Agendamento.Domain.Models;

namespace Agendamento.Domain.Interfaces;

public interface IAgendaRepository : IRepository<Agenda>
{
    // Mantido apenas um método de conflito de horário
    Task<bool> ExisteConflitoHorario(Guid profissionalId, DateTime dataInicio, DateTime dataFim);

    // Métodos novos essenciais para alimentar o AgendadosDto com os relacionamentos carregados
    Task<IEnumerable<Agenda>> ObterTodosComRelacionamentos();
    Task<Agenda> ObterPorIdComRelacionamentos(Guid id);
}
