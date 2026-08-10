using Agendamento.Application.DTOs;
namespace Agendamento.Application.Interfaces;

public interface IPacienteService : IDisposable
{

    Task AdicionarAsync(NovoPacienteDto dto);
    Task AtualizarAsync(AtualizarPacienteDto dto);
    Task RemoverAsync(Guid id);
    Task BloquearAsync(Guid id);
    Task AtivarAsync(Guid id);
}
