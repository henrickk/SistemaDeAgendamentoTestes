using Agendamento.Application.DTOs;
using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Notificacoes;

namespace Agendamento.Application.Services;
public class ProfissionalService : BaseService, IProfissionalService
{
    private readonly IProfissionalRepository _profissionalRepository;
     
    public ProfissionalService(IProfissionalRepository profissionalRepository, INotificador notificador) : base(notificador)
    {
        _profissionalRepository = profissionalRepository;
    }

    public Task AdicionarAsync(NovoProfissionalDto dto)
    {
        throw new NotImplementedException();
    }

    public Task AtivarAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task AtualizarAsync(AtualizarProfissionalDto dto)
    {
        throw new NotImplementedException();
    }

    public Task BloquearAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task RemoverAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
