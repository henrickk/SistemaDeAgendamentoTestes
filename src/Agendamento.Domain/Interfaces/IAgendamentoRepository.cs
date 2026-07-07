using Agendamento.Domain.Models;

namespace Agendamento.Domain.Interfaces
{
    public interface IAgendamentoRepository : IRepository<Agendamento>
    {
        Task AdicionarAsync(Agendamento agendamento);

        Task<bool> ExisteConflitoHorarioAsync(
            Guid profissionalId,
            DateTime dataInicio,
            DateTime dataFim);
    }
}
