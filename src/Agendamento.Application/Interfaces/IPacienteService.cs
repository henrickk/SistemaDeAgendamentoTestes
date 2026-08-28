using Agendamento.Application.DTOs;
namespace Agendamento.Application.Interfaces;

public interface IPacienteService : IDisposable
{
    Task Adicionar(NovoPacienteDto dto);
    Task Atualizar(AtualizarPacienteDto dto);
    Task Remover(Guid id);
    Task Bloquear(Guid id);
    Task Ativar(Guid id);
}
