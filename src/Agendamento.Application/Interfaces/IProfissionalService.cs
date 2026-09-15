using Agendamento.Application.DTOs;
using Agendamento.Domain.Models;

namespace Agendamento.Application.Interfaces;

public interface IProfissionalService : IDisposable
{
    Task AdicionarNovoProfissional(Profissional profissional);
    Task AtualizarProfissional(Profissional profissional);
    Task RemoverProfissional(Guid id);
}
