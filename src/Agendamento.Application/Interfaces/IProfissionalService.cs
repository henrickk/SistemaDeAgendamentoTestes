namespace Agendamento.Application.Interfaces;

public interface IProfissionalService : IDisposable
{
    Task AdicionarAsync(NovoProfissionalDto dto);
    Task AtualizarAsync(AtualizarProfissionalDto dto);
    Task RemoverAsync(Guid id);
    Task BloquearAsync(Guid id);
    Task AtivarAsync(Guid id);
}
