using Agendamento.Application.DTOs;
using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Models.Validations;

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
        var paciente = new Paciente(atualizarPacienteDto.Nome, atualizarPacienteDto.DataDeNascimento, atualizarPacienteDto.StatusGenero,
                                    atualizarPacienteDto.StatusEstadoCivil, atualizarPacienteDto.StatusPaciente, atualizarPacienteDto.Endereco, atualizarPacienteDto.Contato);

    }

    public Task Bloquear(Guid id)
    {
        throw new NotImplementedException();
    }
    public Task Remover(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task Ativar(Guid id)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
