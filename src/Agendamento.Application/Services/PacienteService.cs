using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;

namespace Agendamento.Application.Services;
public class PacienteService : BaseService, IPacienteService
{
    private readonly IPacienteRepository _pacienteRepository;
    private readonly INotificador _notificador;

    public PacienteService(IPacienteRepository pacienteRepository,
                            INotificador notificador) : base(notificador)
    {
        _pacienteRepository = pacienteRepository;
        _notificador = notificador;
    }

    public async Task AdicionarNovoPaciente(Paciente paciente)
    {
        if (string.IsNullOrWhiteSpace(paciente.Nome))
        {
            _notificador.Handle(new Notificacao("O nome é obrigatório."));
            return;
        }

        if (string.IsNullOrWhiteSpace(paciente.Contato.NumeroCelular))
        {
            _notificador.Handle(new Notificacao("O número de celular é obrigatório."));
            return;
        }

        if (string.IsNullOrWhiteSpace(paciente.CPF))
        {
            _notificador.Handle(new Notificacao("O CPF é obrigatório."));
            return;
        }

        var pacienteExistente = await _pacienteRepository.Buscar(p => p.CPF == paciente.CPF);

        if (pacienteExistente.Any())
        {
            Notificar("Já existe um paciente com este CPF.");
            return;
        }

        await _pacienteRepository.Adicionar(paciente);
    }

    public async Task AtualizarInfoPaciente(Paciente paciente)
    {
        if (paciente == null)
        {
            Notificar("Os dados para atualização não podem ser nulos.");
            return;
        }

        var pacienteExistente = await _pacienteRepository.Buscar(p => p.Id == paciente.Id && p.CPF == paciente.CPF);

        if (!pacienteExistente.Any())
        {
            Notificar("Paciente não encontrado.");
            return;
        }

        await _pacienteRepository.Atualizar(paciente);
    }

    public async Task BloquearPaciente(Guid id)
    {
        var pacienteExistente = await _pacienteRepository.ObterPorId(id);
        if (pacienteExistente == null)
        {
            Notificar("Paciente não encontrado.");
            return;
        }

        if (pacienteExistente.StatusPaciente == StatusPaciente.Bloqueado)
        {
            Notificar("Paciente já está bloqueado.");
            return;
        }

        pacienteExistente.StatusPaciente = StatusPaciente.Bloqueado;

        await _pacienteRepository.Atualizar(pacienteExistente);
    }

    public async Task RemoverPaciente(Guid id)
    {
        var pacienteExistente = await _pacienteRepository.ObterPorId(id);

        if (pacienteExistente == null)
        {
            Notificar("Paciente não encontrado.");
            return;
        }

        await _pacienteRepository.Remover(id);
    }

    public async Task AtivarPaciente(Guid id)
    {
        var pacineteExistente = await _pacienteRepository.ObterPorId(id);

        if (pacineteExistente == null)
        {
            Notificar("Paciente não encontrado.");
            return;
        }

        if (pacineteExistente.StatusPaciente == StatusPaciente.Ativo)
        {
            Notificar("Paciente já está ativo.");
            return;
        }

        if (pacineteExistente.StatusPaciente == StatusPaciente.Bloqueado)
        {
            Notificar("Paciente está bloqueado e não pode ser ativado.");
            return;
        }

        pacineteExistente.StatusPaciente = StatusPaciente.Ativo;

        await _pacienteRepository.Atualizar(pacineteExistente);
    }

    public async Task DesativarPaciente(Guid id)
    {
        var pacineteExistente = await _pacienteRepository.ObterPorId(id);

        if (pacineteExistente == null)
        {
            Notificar("Paciente não encontrado.");
            return;
        }

        if (pacineteExistente.StatusPaciente == StatusPaciente.Inativo)
        {
            Notificar("Paciente já está inativo.");
            return;
        }

        if (pacineteExistente.StatusPaciente == StatusPaciente.Bloqueado)
        {
            Notificar("Paciente está bloqueado e não pode ser desativado.");
            return;
        }

        pacineteExistente.StatusPaciente = StatusPaciente.Inativo;

        await _pacienteRepository.Atualizar(pacineteExistente);
    }

    public void Dispose()
    {
        _pacienteRepository?.Dispose();
    }
}
