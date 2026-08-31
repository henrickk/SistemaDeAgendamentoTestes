using Agendamento.Application.DTOs;
namespace Agendamento.Application.Interfaces;

public interface IPacienteService : IDisposable
{
    Task AdicionarNovoPaciente(NovoPacienteDto dto);
    Task AtualizarInfoPaciente(AtualizarPacienteDto dto);
    Task RemoverPaciente(Guid id);
    Task BloquearPaciente(Guid id);
    Task AtivarPaciente(Guid id);
    Task DesativarPaciente(Guid id);
}
