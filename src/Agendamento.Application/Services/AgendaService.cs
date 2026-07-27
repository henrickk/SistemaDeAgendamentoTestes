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

    public async Task CancelarAgendamento(Guid agendamentoId)
    {
        await _agendaRepository.Remover(agendamentoId);
    }

    public Task ConcluirAgendamento(Guid agendamentoId)
    {
        // Criar uma regra de negócio para concluir o agendamento, por exemplo, verificar se o agendamento está no status correto antes de concluir.

        //if (StatusAgendamento.Agendado == StatusAgendamento.Concluido)
        //if()
        //{
        //    agendamentoId = agendamentoId;
        //}
        throw new NotImplementedException();
    }

    public Task ConfirmarAgendamento(Guid agendamentoId)
    {
        // Criar uma regra de negócio para confirmar o agendamento, por exemplo, verificar se o agendamento está no status correto antes de confirmar.
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _agendaRepository.Dispose();

        throw new NotImplementedException();
    }
}
