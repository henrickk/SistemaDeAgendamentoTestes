using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Agendamento.Infrastructure.Repository;
public class AgendaRepository : Repository<Agenda>, IAgendaRepository
{
    private readonly DbContext _context;

    public AgendaRepository(DbContext context) : base()
    {
        _context = context;
    }

    public async Task Adicionar(Agenda agenda)
    {
        await _context.Set<Agenda>().AddAsync(agenda);
        await _context.SaveChangesAsync();''
    }

    public async Task<bool> VerificarConflitoProfissional(Guid profissionalId, DateTime inicio, DateTime fim)
    {
        return await _context.Set<Agenda>()
            .AnyAsync(a => a.ProfissionalId == profissionalId
                           && inicio < a.DataFim
                           && fim > a.DataInicio);
    }

    public Task<bool> ExisteConflitoHorario(Guid profissionalId, DateTime dataInicio, DateTime dataFim)
    {
        throw new NotImplementedException();
    }

    public Task<List<Agenda>> ObterAgendamentosPorProfissional(Guid profissionalId)
    {
        throw new NotImplementedException();
    }
}
