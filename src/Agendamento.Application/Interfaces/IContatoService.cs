using Agendamento.Application.DTOs;

namespace Agendamento.Application.Interfaces;

public interface IContatoService : IDisposable
{
    Task AdicionarContato(NovoPacienteDto dto);
    Task AtualizarContato(AtualizarPacienteDto dto);
    Task RemoverContato(Guid id);
}
