using Agendamento.Application.DTOs;
using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;

namespace Agendamento.Application.Services;
public class ProfissionalService : BaseService, IProfissionalService
{
    private readonly IProfissionalRepository _profissionalRepository;
     
    public ProfissionalService(IProfissionalRepository profissionalRepository, INotificador notificador) : base(notificador)
    {
        _profissionalRepository = profissionalRepository;
    }

    public async Task AdicionarProfissional(Profissional profissional)
    {
        var profissionalExistente = await _profissionalRepository.Buscar(p => p.CRO == profissional.CRO);

        if (profissionalExistente.Any())
        {
            Notificar("Já existe um profissional com este CRO.");
            return;
        }

        await _profissionalRepository.Adicionar(profissional);
        await _profissionalRepository.SaveChanges();
    }

    public Task AtivarProfissional(Guid id)
    {
        var profissional = _profissionalRepository.ObterPorId(id).Result;

        if (profissional == null)
        {
            Notificar("Profissional não encontrado.");
            return Task.CompletedTask;
        }

        _profissionalRepository.Atualizar(Profissional);
        return Task.CompletedTask;
    }

    public Task AtualizarProfissional(AtualizarProfissionalDto dto)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task RemoverProfissional(Guid id)
    {
        throw new NotImplementedException();
    }
}
