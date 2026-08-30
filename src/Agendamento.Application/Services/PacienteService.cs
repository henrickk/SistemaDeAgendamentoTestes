using Agendamento.Application.DTOs;
using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;

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
    public async Task Adicionar(NovoPacienteDto novoPacienteDto)
    {
        var paciente = new Paciente(novoPacienteDto.Nome, novoPacienteDto.DataDeNascimento, novoPacienteDto.CPF, novoPacienteDto.RG, novoPacienteDto.StatusGenero,
                                    novoPacienteDto.StatusEstadoCivil, novoPacienteDto.StatusPaciente, novoPacienteDto.Endereco, novoPacienteDto.Contato);

        await _pacienteRepository.Adicionar(paciente);
    }
    public async Task Atualizar(AtualizarPacienteDto atualizarPacienteDto)
    {
        if (atualizarPacienteDto == null)
        {
            Notificar("Os dados para atualização não podem ser nulos.");
            return;
        }

        var pacienteExistente = await _pacienteRepository.Buscar(p => p.Id == atualizarPacienteDto.Id);

        if (pacienteExistente == null || !pacienteExistente.Any())
        {
            Notificar("Paciente não encontrado.");
            return;
        }

        var paciente = pacienteExistente.First();
        paciente.Nome = atualizarPacienteDto.Nome;
        paciente.DataNascimento = atualizarPacienteDto.DataDeNascimento;
        paciente.StatusGenero = atualizarPacienteDto.StatusGenero;
        paciente.StatusEstadoCivil = atualizarPacienteDto.StatusEstadoCivil;
        paciente.StatusPaciente = atualizarPacienteDto.StatusPaciente;
        paciente.Endereco = atualizarPacienteDto.Endereco;
        paciente.Contato = atualizarPacienteDto.Contato;

        await _pacienteRepository.Atualizar(paciente);
    }

    public async Task Bloquear(Guid id)
    {
        var pacienteExistente = await _pacienteRepository.ObterPorId(id);
        if (pacienteExistente == null)
        {
            Notificar("Paciente não encontrado.");
            return;
        }

        pacienteExistente.StatusPaciente = StatusPaciente.Bloqueado;

        await _pacienteRepository.Atualizar(pacienteExistente);
    }
    public async Task Remover(Guid id)
    {
        var pacienteExistente = await _pacienteRepository.ObterPorId(id);

        if (pacienteExistente == null)
        {
            Notificar("Paciente não encontrado.");
            return;
        }

        await _pacienteRepository.Remover(id);
    }

    public async Task Ativar(Guid id)
    {
        var pacineteExistente = await _pacienteRepository.ObterPorId(id);

        pacineteExistente.StatusPaciente = StatusPaciente.Ativo;

        await _pacienteRepository.Atualizar(pacineteExistente);
    }

    public async Task Desativar(Guid id)
    {
        var pacineteExistente = await _pacienteRepository.ObterPorId(id);

        pacineteExistente.StatusPaciente = StatusPaciente.Inativo;

        await _pacienteRepository.Atualizar(pacineteExistente);
    }

    public void Dispose()
    {
        _pacienteRepository?.Dispose();
    }
}
