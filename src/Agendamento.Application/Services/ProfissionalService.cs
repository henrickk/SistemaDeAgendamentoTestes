using Agendamento.Application.DTOs;
using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;

namespace Agendamento.Application.Services;
public class ProfissionalService : BaseService, IProfissionalService
{
    private readonly IProfissionalRepository _profissionalRepository;

    public ProfissionalService(IProfissionalRepository profissionalRepository, INotificador notificador) : base(notificador)
    {
        _profissionalRepository = profissionalRepository;
    }

    public async Task AdicionarNovoProfissional(Profissional profissional)
    {
        var profissionalExistente = await _profissionalRepository.Buscar(p => p.CRO == profissional.CRO);

        if (profissionalExistente == null)
        {
            Notificar("Profissional não encontrado.");
            return;
        }

        if (profissionalExistente.Any())
        {
            Notificar("Já existe um profissional com este CRO.");
            return;
        }

        await _profissionalRepository.Adicionar(profissional);
        await _profissionalRepository.SaveChanges();
    }

    public async Task AtualizarProfissional(Profissional profissional)
    {
        var profissionalExistente = await _profissionalRepository.ObterPorId(profissional.Id);

        if (profissionalExistente == null)
        {
            Notificar("Profissional não encontrado.");
            return;
        }

        _profissionalRepository.Atualizar(profissional);
        await _profissionalRepository.SaveChanges();
    }

    public async Task RemoverProfissional(Guid id)
    {
        var profissional = await _profissionalRepository.ObterPorId(id);

        if (profissional == null)
        {
            Notificar("Profissional não encontrado.");
            return;
        }
        await _profissionalRepository.Remover(id);
    }

    public void Dispose()
    {
        _profissionalRepository?.Dispose();
    }
}
