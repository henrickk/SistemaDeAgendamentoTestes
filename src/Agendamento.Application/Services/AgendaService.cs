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
                         IProfissionalRepository profissionalRepository) : base(notificador)
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
            Notificar("Paciente bloqueado. Não é possível realizar agendamento.");
            return;
        }

        var profissional = await _profissionalRepository.ObterPorId(novoAgendamentoDto.ProfissionalId);

        if (profissional == null)
        {
            Notificar("Profissional não encontrado.");
            return;
        }

        var possuiConflito = await _agendaRepository.ExisteConflitoHorario(
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
        await _agendaRepository.SaveChanges();
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
        await _agendaRepository.SaveChanges();
    }

    public async Task ConcluirAgendamento(Guid agendamentoId)
    {
        var agendamento = await _agendaRepository.ObterPorId(agendamentoId);
        if (agendamento == null)
        {
            Notificar("Agendamento não encontrado.");
            return;
        }
        if (agendamento.StatusAgendamento == StatusAgendamento.Concluido)
        {
            Notificar("O agendamento já está concluído.");
            return;
        }
        agendamento.StatusAgendamento = StatusAgendamento.Concluido;
        await _agendaRepository.Atualizar(agendamento);
        await _agendaRepository.SaveChanges();
    }

    public async Task ConfirmarAgendamento(Guid agendamentoId)
    {
        var agendamento = await _agendaRepository.ObterPorId(agendamentoId);

        // Verificar se agendamento existe
        if (agendamento == null)
        {
            Notificar("Agendamento não encontrado.");
            return;
        }

        if (agendamento.StatusAgendamento == StatusAgendamento.Agendado)
        {
            agendamento.StatusAgendamento = StatusAgendamento.Confirmado; // Corrigido para alterar o status diretamente
            await _agendaRepository.Atualizar(agendamento);
            return;
        }

        var paciente = await _pacienteRepository.ObterPorId(agendamento.PacienteId);

        if (paciente == null)
        {
            Notificar("Paciente não encontrado.");
            return;
        }

        // verificar se o agendamento já está confirmado
        if (agendamento.StatusAgendamento == StatusAgendamento.Confirmado)
        {
            Notificar("O agendamento já está confirmado.");
            return;
        }

        // verificar se o paciente está bloqueado
        if (paciente.StatusPaciente == StatusPaciente.Bloqueado)
        {
            Notificar("Paciente bloqueado. Não é possível confirmar agendamento.");
            return;
        }

        agendamento.StatusAgendamento = StatusAgendamento.Confirmado;
        await _agendaRepository.Atualizar(agendamento);
        await _agendaRepository.SaveChanges();
    }

    public void Dispose()
    {
        _agendaRepository?.Dispose();
    }
}
