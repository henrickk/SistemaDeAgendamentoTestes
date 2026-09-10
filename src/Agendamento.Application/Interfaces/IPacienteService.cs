using Agendamento.Application.DTOs;
using Agendamento.Domain.Models;
namespace Agendamento.Application.Interfaces;

public interface IPacienteService : IDisposable
{
    Task AdicionarNovoPaciente(Paciente paciente);
    Task AtualizarInfoPaciente(Paciente paciente);
    Task RemoverPaciente(Guid id);
    Task BloquearPaciente(Guid id);
    Task AtivarPaciente(Guid id);
    Task DesativarPaciente(Guid id);
}
