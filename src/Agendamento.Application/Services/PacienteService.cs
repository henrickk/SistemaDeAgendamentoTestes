using Agendamento.Application.DTOs;
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

    public async Task AdicionarNovoPaciente(NovoPacienteDto novoPacienteDto)
    {
        if (string.IsNullOrWhiteSpace(novoPacienteDto.Nome))
        {
            _notificador.Handle(new Notificacao("O nome é obrigatório."));
            return;
        }

        if (string.IsNullOrWhiteSpace(novoPacienteDto.Contato.NumeroCelular))
        {
            _notificador.Handle(new Notificacao("O número de celular é obrigatório."));
            return;
        }

        if (string.IsNullOrWhiteSpace(novoPacienteDto.CPF))
        {
            _notificador.Handle(new Notificacao("O CPF é obrigatório."));
            return;
        }

        var paciente = new Paciente(novoPacienteDto.Nome,
                                    novoPacienteDto.DataDeNascimento,
                                    novoPacienteDto.CPF,
                                    novoPacienteDto.RG,
                                    novoPacienteDto.StatusGenero,
                                    novoPacienteDto.StatusEstadoCivil,
                                    novoPacienteDto.StatusPaciente,
                                    novoPacienteDto.Endereco,
                                    novoPacienteDto.Contato
        );

        await _pacienteRepository.Adicionar(paciente);
    }

    public async Task AtualizarInfoPaciente(AtualizarPacienteDto atualizarPacienteDto)
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

        pacineteExistente.StatusPaciente = StatusPaciente.Ativo;

        await _pacienteRepository.Atualizar(pacineteExistente);
    }

    public async Task DesativarPaciente(Guid id)
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
