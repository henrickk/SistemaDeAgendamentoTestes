using Agendamento.Application.DTOs;
using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;

namespace Agendamento.Application.Services;
public class AgendaService : BaseService, IAgendaService
{
    private readonly IAgendaRepository _agendaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IProfissionalRepository _profissionalRepository;
    public AgendaService(INotificador notificador,
                         IAgendaRepository agendaRepository,
                         IPacienteRepository pacienteRepository,
                         IProfissionalRepository profissionalRepository) : base(notificador
        )
    {
        _agendaRepository = agendaRepository;
        _pacienteRepository = pacienteRepository;
        _profissionalRepository = profissionalRepository;
    }

    public async Task Agendar(NovoAgendamentoDto novoAgendamentoDto)
    {
        var paciente = await _pacienteRepository.ObterPorId(novoAgendamentoDto.PacienteId);

        if (paciente == null)
        {
            Notificar("Paciente não encontrado.");
            return;
        }

        if (paciente.StatusPaciente == StatusPaciente.Bloqueado)
        {
            // Certifique-se de que o método interno do seu BaseService 
            // realmente cria e envia o objeto Notificacao esperado.
            Notificar("Paciente bloqueado. Não é possível realizar agendamento.");
            return;
        }

        var profissional = await _profissionalRepository.ObterPorId(novoAgendamentoDto.ProfissionalId);

        if (profissional == null)
        {
            Notificar("Profissional não encontrado.");
            return;
        }

        // 🔴 NOVA VALIDAÇÃO: Verificar conflito de horário
        var possuiConflito = await _agendaRepository.VerificarConflitoProfissional(
            novoAgendamentoDto.ProfissionalId,
            novoAgendamentoDto.DataInicio,
            novoAgendamentoDto.DataFim
        );

        if (possuiConflito)
        {
            Notificar("O profissional já possui um agendamento neste horário.");
            return;
        }

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
        var agendamento = await _agendaRepository.ObterPorId(agendamentoId);

        if (agendamento == null)
        {
            Notificar("Agendamento não encontrado.");
            return;
        }

        if (agendamento.StatusAgendamento == StatusAgendamento.Cancelado)
        {
            Notificar("O agendamento já está cancelado.");
            return;
        }

        agendamento.StatusAgendamento = StatusAgendamento.Cancelado;
        await _agendaRepository.Atualizar(agendamento);
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
