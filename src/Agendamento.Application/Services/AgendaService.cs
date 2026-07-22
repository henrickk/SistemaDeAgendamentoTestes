using Agendamento.Application.DTOs;
using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;

namespace Agendamento.Application.Services;
public class AgendaService : BaseService, IAgendaService
{
    private readonly IAgendaRepository _agendaRepository;
    public AgendaService(INotificador notificador, IAgendaRepository agendaRepository) : base(notificador)
    {
        _agendaRepository = agendaRepository;
    }

    public async Task Agendar(NovoAgendamentoDto novoAgendamentoDto)
    {
        var agendamento = new Agenda(
        novoAgendamentoDto.PacienteId,
        novoAgendamentoDto.ProfissionalId,
        StatusAgendamento.Agendado,
        novoAgendamentoDto.DataInicio,
        novoAgendamentoDto.DataFim,
        novoAgendamentoDto.Observacao,
        null,
        null);

        await _agendaRepository.Adicionar(agendamento);
    }

    public Task CancelarAgendamento(Guid agendamentoId)
    {
        throw new NotImplementedException();
    }

    public Task ConcluirAgendamento(Guid agendamentoId)
    {
        throw new NotImplementedException();
    }

    public Task ConfirmarAgendamento(Guid agendamentoId)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
