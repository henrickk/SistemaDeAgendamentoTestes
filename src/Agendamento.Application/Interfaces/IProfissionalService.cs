using Agendamento.Application.DTOs;
using Agendamento.Domain.Models;

namespace Agendamento.Application.Interfaces;

public interface IProfissionalService : IDisposable
{
    Task AdicionarProfissional(Profissional profissional);
    Task AtualizarProfissional(AtualizarProfissionalDto dto);
    Task RemoverProfissional(Guid id);
    Task AtivarProfissional(Guid id);
}
