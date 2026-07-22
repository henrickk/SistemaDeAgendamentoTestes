using Agendamento.Application.DTOs;

namespace Agendamento.Application.Interfaces;

public interface IAgendaService : IDisposable
{
    Task Agendar(NovoAgendamentoDto novoAgendamentoDto);

    Task ConfirmarAgendamento(Guid agendamentoId);

    Task CancelarAgendamento(Guid agendamentoId);

    Task ConcluirAgendamento(Guid agendamentoId);
}
